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
        public static List<aszIAction> TranslateFromJson(string json)
        {
            JsonDocument doc = JsonDocument.Parse(json);
            List<aszIAction> actions = new List<aszIAction>();

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

        private static aszActionWalk CreateWalkAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            float stop = element.TryGetProperty("stopDistance", out JsonElement s) ? s.GetSingle() : 0.5f;
            return new aszActionWalk(target, stop);
        }
        private static aszActionReach CreateReachAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            string hand = element.TryGetProperty("hand", out JsonElement s) ? s.GetString() : "Right";
            return new aszActionReach(target, hand);
        }
        private static aszActionHold CreatHoldAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new aszActionHold(target);
        }
        private static aszActionTouch CreateTouchAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            string hand = element.TryGetProperty("hand", out JsonElement s) ? s.GetString() : "Right";
            return new aszActionTouch(target, hand);
        }
        private static aszActionSit CreateSitAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new aszActionSit(target);
        }
        private static aszActionHug CreateHugAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new aszActionHug(target);
        }
        private static aszActionKiss CreateKissAction(JsonElement element)
        {
            string target = element.GetProperty("target").GetString();
            return new aszActionKiss(target);
        }
    }
}
