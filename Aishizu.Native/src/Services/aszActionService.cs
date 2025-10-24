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
            aszLogger.WriteLine($"PPAP");
            aszLogger.WriteLine($"[aszActionService] Instance HashCode: {GetHashCode()} Count: {m_JsonSchemas.Count}");
            foreach (string s in m_JsonSchemas.Values)
            {
                aszLogger.WriteLine($"[aszActionService] Action Json: {s}");
            }
            return JsonSerializer.Serialize(m_JsonSchemas, options);
        }

        public aszIAction JsonToAction(string actionName, string json)
        {
            if (!m_ActionTypes.TryGetValue(actionName, out Type type))
            {
                aszLogger.WriteLine($"[aszActionService] Unknown action type: {actionName}");
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
    }
}
