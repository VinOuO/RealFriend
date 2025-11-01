using Aishizu.Native.Events;
using Aishizu.Native.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Aishizu.Native
{
    public class aszSequenceService
    {
        private Queue<aszIEvent> m_Sequence = new();
        private List<aszAction> m_Actions = new List<aszAction>();
        private aszIEvent? m_Current;
        private bool m_IsRunning; public bool IsRunning => m_IsRunning;
        private bool m_IsPaused; public bool IsPaused => m_IsPaused;
        private bool m_IsFinished; public bool IsFinished => m_IsFinished;

        public Result JsonToSequence(string json, out List<aszIEvent> eventList)
        {
            eventList = new List<aszIEvent>();
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("Events", out JsonElement eventsArray))
                {
                    aszLogger.WriteLine("[aszActionService] No 'Events' found in JSON.");
                    return Result.Failed;
                }

                foreach (var eventElement in eventsArray.EnumerateArray())
                {
                    string type = eventElement.GetProperty("Type").GetString();

                    switch (type)
                    {
                        case "ActionBegin":
                            eventList.Add(new aszActionBegin
                            {
                                actionId = eventElement.GetProperty("ActionId").GetInt32()
                            });
                            break;

                        case "ActionEnd":
                            eventList.Add(new aszActionEnd
                            {
                                actionId = eventElement.GetProperty("ActionId").GetInt32()
                            });
                            break;

                        case "Wait":
                            eventList.Add(new aszWait
                            {
                                Duration = eventElement.GetProperty("Duration").GetSingle()
                            });
                            break;

                        default:
                            aszLogger.WriteLine($"[aszActionService] Unknown event type: {type}");
                            break;
                    }
                }
                return Result.Success;
            }
            catch (Exception ex)
            {
                aszLogger.WriteLine($"[aszActionService] JsonToSequence failed: {ex.Message}");
                return Result.Failed;
            }
        }

        public void Init(List<aszAction> actions, List<aszIEvent> events) 
        {
            for (int i = 0; i < events.Count; i++)
            {
                Enqueue(events[i]);
            }
            m_IsRunning = false;
            m_IsPaused = false;
            m_IsFinished = false;
            m_Actions = actions;
            m_Current = null;
        }

        public void Enqueue(aszIEvent asz_Event)
        {
            m_Sequence.Enqueue(asz_Event);
        }

        public void Start()
        {
            if (m_IsRunning)
            {
                return;
            }
            m_IsRunning = true;
            NextAction();
        }

        public void Kill()
        {
            m_IsRunning = false;
            m_Sequence.Clear();
            m_Current = null;
            m_IsPaused = false;
        }

        public void Pause(bool pause)
        {
            if (m_IsRunning)
            {
                m_IsPaused = pause;
            }
            else
            {
                m_IsPaused = false;
            }
        }

        public void Tick(float deltaTime)
        {
            if (!m_IsRunning || m_IsPaused || m_Current == null)
            {
                return;
            }

            switch (m_Current)
            {
                case aszActionBegin actionBegin:
                    m_Actions[actionBegin.actionId].Update(deltaTime);
                    if(m_Actions[actionBegin.actionId].State == aszActionState.Running)
                    {
                        NextAction();
                    }
                    if (m_Actions[actionBegin.actionId].IsFinished)
                    {
                        NextAction();
                    }
                    break;
                case aszActionEnd asctionEnd:
                    m_Actions[asctionEnd.actionId].Update(deltaTime);
                    if (m_Actions[asctionEnd.actionId].IsFinished)
                    {
                        NextAction();
                    }
                    break;
                case  aszWait wait:
                    wait.Duration -= deltaTime;
                    aszLogger.WriteLine($"[aszSequenceServices] Remainning waitting time: {wait.Duration}");
                    if (wait.Duration < 0)
                    {
                        NextAction();
                    }
                    break;
            }
        }

        private void NextAction()
        {
            if (m_Sequence.Count > 0)
            {
                m_Current = m_Sequence.Dequeue();

                switch (m_Current)
                {
                    case aszActionBegin actionBegin:
                        aszLogger.WriteLine($"[aszSequenceServices] Beginning: {m_Actions[actionBegin.actionId].ActionName}");
                        m_Actions[actionBegin.actionId].Start();
                        break;
                    case aszActionEnd actionEnd:
                        aszLogger.WriteLine($"[aszSequenceServices] Finishing: {m_Actions[actionEnd.actionId].ActionName}");
                        m_Actions[actionEnd.actionId].Finish();
                        if (m_Actions[actionEnd.actionId].State == aszActionState.Failed)
                        {
                            aszLogger.WriteLine($"[aszSequenceServices]: {m_Actions[actionEnd.actionId].ActionName} skipped finishing due to failed status");
                            NextAction();
                            return;
                        }
                        break;
                }
            }
            else
            {
                m_IsRunning = false;
                m_Current = null;
                m_IsFinished = true;
            }
        }
    }
}
