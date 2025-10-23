using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Aishizu.Native.Actions
{
    public class aszActionService
    {
        private readonly Dictionary<string, Type> m_ActionTypes = new();
        private readonly Dictionary<string, string> m_JsonSchemas = new();

        // Developer manually registers actions
        public void RegisterAction<T>() where T : aszIAction, new()
        {
            T instance = new T();
            string actionName = typeof(T).Name.Replace("asz", string.Empty);

            if (m_ActionTypes.ContainsKey(actionName))
            {
                Console.WriteLine($"[aszActionService] Action '{actionName}' already registered.");
                return;
            }

            m_ActionTypes[actionName] = typeof(T);
            m_JsonSchemas[actionName] = instance.ToJson();

            Console.WriteLine($"[aszActionService] Registered action: {actionName}");
        }

        // Get schema list for LLM (as dictionary)
        public Dictionary<string, string> GetActionSchemas()
        {
            return m_JsonSchemas;
        }

        // ✅ New function: Get schema list as serialized JSON string
        public string GetActionSchemasAsJson()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };

            return JsonSerializer.Serialize(m_JsonSchemas, options);
        }

        // Parse LLM JSON → Unity action
        public aszIAction JsonToAction(string actionName, string json)
        {
            if (!m_ActionTypes.TryGetValue(actionName, out Type type))
            {
                Console.WriteLine($"[aszActionService] Unknown action type: {actionName}");
                return null;
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            };

            aszIAction instance = (aszIAction)JsonSerializer.Deserialize(json, type, options);
            return instance;
        }

        // Optional: serialize Unity action → JSON
        public string ActionToJson(aszIAction action)
        {
            return action.ToJson();
        }
    }
}
