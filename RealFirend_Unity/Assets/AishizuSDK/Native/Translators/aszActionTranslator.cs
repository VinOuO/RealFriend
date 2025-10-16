using System.Text.Json;
using System.Collections.Generic;

namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Translates AI-generated JSON into Aishizu Native Actions.
    /// Acts as the semantic bridge between AI intent and virtual behavior.
    /// </summary>
    public static class aszActionTranslator
    {
        public static List<IAction> TranslateFromJson(string json)
        {
            JsonDocument doc = JsonDocument.Parse(json);
            List<IAction> actions = new List<IAction>();

            if (!doc.RootElement.TryGetProperty("actions", out JsonElement actionArray))
                return actions;

            foreach (JsonElement element in actionArray.EnumerateArray())
            {
                if (!element.TryGetProperty("type", out JsonElement typeProp))
                    continue;

                string type = typeProp.GetString();

                switch (type)
                {
                    case "Walk":
                        actions.Add(CreateWalkAction(element));
                        break;
                    case "Reach":
                        actions.Add(CreateReachAction(element));
                        break;
                    case "Hold":
                        actions.Add(CreatHoldAction(element));
                        break;
                    case "Touch":
                        actions.Add(CreateTouchAction(element));
                        break;
                    case "Sit":
                        actions.Add(CreateSitAction(element));
                        break;
                    case "Hug":
                        actions.Add(CreateHugAction(element));
                        break;
                    case "Kiss":
                        actions.Add(CreateKissAction(element));
                        break;
                }
            }

            return actions;
        }

        private static ActionWalk CreateWalkAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            float stop = element.TryGetProperty("stopDistance", out JsonElement s) ? s.GetSingle() : 0.5f;
            return new ActionWalk(target, stop);
        }
        private static ActionReach CreateReachAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            string hand = element.TryGetProperty("hand", out JsonElement s) ? s.GetString() : "Right";
            return new ActionReach(target, hand);
        }
        private static ActionHold CreatHoldAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new ActionHold(target);
        }
        private static ActionTouch CreateTouchAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            string hand = element.TryGetProperty("hand", out JsonElement s) ? s.GetString() : "Right";
            return new ActionTouch(target, hand);
        }
        private static ActionSit CreateSitAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new ActionSit(target);
        }
        private static ActionHug CreateHugAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new ActionHug(target);
        }
        private static ActionKiss CreateKissAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new ActionKiss(target);
        }
    }
}
