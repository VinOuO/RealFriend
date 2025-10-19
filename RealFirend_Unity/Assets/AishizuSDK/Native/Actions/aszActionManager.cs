using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aishizu.Native.Actions
{
    public class aszActionManager
    {
        private readonly Dictionary<string, Type> m_ActionTypes = new();
        private readonly Dictionary<string, string> m_JsonSchemas = new();

        // 🔹 Developer manually registers actions
        public void RegisterAction<T>() where T : aszIAction, new()
        {
            T instance = new T();
            string actionName = typeof(T).Name.Replace("asz", string.Empty);

            if (m_ActionTypes.ContainsKey(actionName))
            {
                Debug.LogWarning($"[aszActionManager] Action '{actionName}' already registered.");
                return;
            }

            m_ActionTypes[actionName] = typeof(T);
            m_JsonSchemas[actionName] = JsonUtility.ToJson(instance, true);

            Debug.Log($"[aszActionManager] Registered action: {actionName}");
        }


        // 📤 Get schema list for LLM
        public Dictionary<string, string> GetActionSchemas() => m_JsonSchemas;

        // 📥 Parse LLM JSON → Unity action
        public aszIAction JsonToAction(string actionName, string json)
        {
            if (!m_ActionTypes.TryGetValue(actionName, out var type))
            {
                Debug.LogError($"[aszActionManager] Unknown action type: {actionName}");
                return null;
            }

            var instance = (aszIAction)Activator.CreateInstance(type);
            JsonUtility.FromJsonOverwrite(json, instance);
            return instance;
        }

        // Optional: serialize Unity action → JSON
        public string ActionToJson(aszIAction action)
        {
            return JsonUtility.ToJson(action, true);
        }
    }
}
