using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Aishizu.Native.Actions
{
    public class aszActionService
    {
        private readonly Dictionary<string, Type> m_ActionTypes = new();
        private readonly Dictionary<string, string> m_JsonSchemas = new();

        public void RegisterAction<T>() where T : aszIAction, new()
        {
            T instance = new T();
            string actionName = typeof(T).Name.Replace("asz", string.Empty);

            if (m_ActionTypes.ContainsKey(actionName))
            {
                Console.WriteLine($"[aszActionService] Action '{actionName}' already registered.");
                return;
            }

            m_ActionTypes.Add(actionName, typeof(T));
            m_JsonSchemas.Add(actionName, instance.ToJson());
            aszLogger.WriteLine($"[aszActionService] Instance HashCode: {GetHashCode()} Count: {m_JsonSchemas.Count}");
            aszLogger.WriteLine($"[aszActionService] Registered action: {actionName}");
            aszLogger.WriteLine($"[aszActionService] Action Json: {m_JsonSchemas[actionName]}");
        }

        public Dictionary<string, string> GetActionSchemas()
        {
            return m_JsonSchemas;
        }

        public string GetActionSchemasAsJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };
            aszLogger.WriteLine($"[aszActionService] Instance HashCode: {GetHashCode()} Count: {m_JsonSchemas.Count}");
            foreach (string s in m_JsonSchemas.Values)
            {
                aszLogger.WriteLine($"[aszActionService] Action Json: {s}");
            }
            return JsonSerializer.Serialize(m_JsonSchemas, options);
        }


        public Result JsonToActions(string json, out List<aszAction> result)
        {
            result = new List<aszAction>();

            using JsonDocument doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("Actions", out JsonElement actionsArray))
                return Result.Failed;

            foreach (JsonElement actionElement in actionsArray.EnumerateArray())
            {
                string actionName = actionElement.GetProperty("ActionName").GetString();
                Type actionType = GetRegisteredActionType(actionName);

                if (actionType == null)
                {
                    aszLogger.WriteLine($"Unknown action: {actionName}");
                    continue;
                }

                try
                {
                    aszAction action = (aszAction)JsonSerializer.Deserialize(
                        actionElement.GetRawText(),
                        actionType,
                        aszJsonSettings.DefaultJsonOptions
                    );
                    result.Add(action);
                }
                catch (Exception ex)
                {
                    aszLogger.WriteLine($"Failed to deserialize {actionName}: {ex.Message}");
                    aszLogger.WriteLine($"Json: {actionElement.GetRawText()}");
                }
            }

            return result.Count > 0 ? Result.Success : Result.Failed;
        }

        /*
        public Result JsonToActions(string json, out List<aszAction> result)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            result = new List<aszAction>();
            if (!root.TryGetProperty("Actions", out JsonElement actionsArray))
            {
                return Result.Failed;
            }

            foreach (JsonElement actionElement in actionsArray.EnumerateArray())
            {
                string? actionName = actionElement.GetProperty("ActionName").GetString();
                int actorId = actionElement.GetProperty("ActorId").GetInt32();
                int targetId = actionElement.GetProperty("TargetId").GetInt32();
                bool undo = actionElement.GetProperty("Undo").GetBoolean();

                Type actionType = GetRegisteredActionType(actionName == null ? "" : actionName);
                if (actionType == null)
                {
                    aszLogger.WriteLine($"Unknown action: {actionName}");
                    continue;
                }

                if (Activator.CreateInstance(actionType) is not aszAction actionInstance)
                {
                    continue;
                }
                actionInstance.ActorId = actorId;
                actionInstance.TargetId = targetId;
                actionInstance.Undo = undo;
                result.Add(actionInstance);
            }
            return Result.Success;
        }
        */
        public Type GetRegisteredActionType(string actionName)
        {
            m_ActionTypes.TryGetValue(actionName, out Type type);
            return type;
        }
    }
}
