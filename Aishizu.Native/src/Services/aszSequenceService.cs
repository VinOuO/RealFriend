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

        public event Action<aszAction> OnActionStart = delegate { };
        public Action<aszAction> OnActionUpdate = delegate { };
        public Action<aszAction> OnActionFinish = delegate { };
        public Action<aszEmotionChange> OnEmotionChange = delegate { };

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

                        case "EmotionChange":
                            aszEmotionChange? emotionChange = (aszEmotionChange?)JsonSerializer.Deserialize(eventElement, typeof(aszEmotionChange), aszJsonSettings.DefaultJsonOptions);
                            if (emotionChange != null)
                            {
                                eventList.Add(emotionChange); ;
                            }
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
            NextEvent();
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
                    OnActionUpdate(m_Actions[actionBegin.actionId]);
                    if (m_Actions[actionBegin.actionId].State == aszActionState.Running)
                    {
                        NextEvent();
                    }
                    if (m_Actions[actionBegin.actionId].IsFinished)
                    {
                        NextEvent();
                    }
                    break;
                case aszActionEnd actionEnd:
                    OnActionUpdate(m_Actions[actionEnd.actionId]);
                    if (m_Actions[actionEnd.actionId].IsFinished)
                    {
                        NextEvent();
                    }
                    break;
                case aszEmotionChange emotionChange:
                    OnEmotionChange(emotionChange);
                    NextEvent();
                    break;
                case  aszWait wait:
                    wait.Duration -= deltaTime;
                    aszLogger.WriteLine($"[aszSequenceServices] Remainning waitting time: {wait.Duration}");
                    if (wait.Duration < 0)
                    {
                        NextEvent();
                    }
                    break;
            }
        }

        private void NextEvent()
        {
            if (m_Sequence.Count > 0)
            {
                m_Current = m_Sequence.Dequeue();

                switch (m_Current)
                {
                    case aszActionBegin actionBegin:
                        aszLogger.WriteLine($"[aszSequenceServices] Beginning: {m_Actions[actionBegin.actionId].ActionName}");
                        OnActionStart(m_Actions[actionBegin.actionId]);
                        break;
                    case aszActionEnd actionEnd:
                        aszLogger.WriteLine($"[aszSequenceServices] Finishing: {m_Actions[actionEnd.actionId].ActionName}");
                        OnActionFinish(m_Actions[actionEnd.actionId]);
                        if (m_Actions[actionEnd.actionId].State == aszActionState.Failed)
                        {
                            aszLogger.WriteLine($"[aszSequenceServices]: {m_Actions[actionEnd.actionId].ActionName} skipped finishing due to failed status");
                            NextEvent();
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
