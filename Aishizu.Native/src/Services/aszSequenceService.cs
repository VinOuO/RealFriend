using Aishizu.Native.Events;
using Aishizu.Native.Actions;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Aishizu.Native.Services
{
    public class aszSequenceService
    {
        private Queue<aszIEvent> m_Sequence = new(); public Queue<aszIEvent> Sequence => m_Sequence;
        private List<aszAction> m_Actions = new List<aszAction>(); public List<aszAction> Actions => m_Actions;
        public Result JsonToSequence(string json, out List<aszIEvent> eventList)
        {
            eventList = new List<aszIEvent>();
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("Events", out JsonElement eventsArray))
                {
                    aszLogger.WriteLine("[aszSequenceService] No 'Events' found in JSON.");
                    return Result.Failed;
                }

                foreach (JsonElement eventElement in eventsArray.EnumerateArray())
                {
                    string? type = eventElement.GetProperty("Type").GetString();
                    if(type == null)
                    {
                        aszLogger.WriteLine($"[aszSequenceService] null event type");
                        continue;
                    }

                    switch (type)
                    {
                        case "ActionBegin":
                            eventList.Add(new aszActionStart
                            {
                                actionId = eventElement.GetProperty("ActionId").GetInt32(),
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
                aszLogger.WriteLine($"[aszSequenceService] JsonToSequence Finished");
                return Result.Success;
            }
            catch (Exception ex)
            {
                aszLogger.WriteLine($"[aszSequenceService] JsonToSequence failed: {ex.Message}");
                return Result.Failed;
            }
        }

        public Queue<aszIEvent> GenerateSequence(List<aszAction> actions, List<aszIEvent> events) 
        {
            for (int i = 0; i < events.Count; i++)
            {
                switch (events[i])
                {
                    case aszActionStart actionStart:
                        if (actionStart.actionId >= 0 && actionStart.actionId < actions.Count)
                        {
                            actionStart.Name = "Start" + actions[actionStart.actionId].ActionName;
                        }
                        break;
                    case aszActionEnd actionEnd:
                        if (actionEnd.actionId >= 0 && actionEnd.actionId < actions.Count)
                        {
                            actionEnd.Name = "End" + actions[actionEnd.actionId].ActionName;
                        }
                        break;
                }
                aszLogger.WriteLine($"[aszSequenceService] Queued Event: {events[i].Name}");
                Enqueue(events[i]);
            }
            m_Actions = actions;
            return m_Sequence;
        }

        private void Enqueue(aszIEvent asz_Event)
        {
            m_Sequence.Enqueue(asz_Event);
        }
    }
}
