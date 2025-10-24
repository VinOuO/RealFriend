using UnityEngine;
using Aishizu.Native.Services;
using Aishizu.Native.Actions;
using System.Net.Http;
using System;

namespace Aishizu.UnityCore
{
    [RequireComponent(typeof(aszActorManager))]
    [RequireComponent(typeof(aszInterableManager))]
    public class aszTheater : MonoBehaviour
    {
        public static aszTheater Instance;
        private aszAIMediator m_AIMediator; 
        private aszActorManager m_ActorManager; public aszActorManager ActorManager => m_ActorManager;
        private aszInterableManager m_InterableManager; public aszInterableManager InterableManager => m_InterableManager;

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            Instance = this;
            m_ActorManager = GetComponent<aszActorManager>();
            m_InterableManager = GetComponent<aszInterableManager>();
        }

        [ContextMenu("InitializeMediator")]
        public void InitializeMediator()
        {
            Init();
            m_AIMediator = new aszAIMediator();
            m_AIMediator.Endpoint = "http://localhost:1234/v1/chat/completions";
            m_AIMediator.Model = "Mythomax L2 13B";
            #region Register Actors
            for (int i = 0; i < m_ActorManager.ActorList.Count; i++)
            {
                m_AIMediator.ActorService.RegisterActor(i, m_ActorManager.ActorList[i].name);
            }
            #endregion
            #region Register Interables
            for (int i = 0; i < m_InterableManager.InterableList.Count; i++)
            {
                m_AIMediator.TargetService.RegisterTarget(i, m_InterableManager.InterableList[i].name);
            }
            #endregion
            Debug.Log("[AishizuCore] Mediator and services initialized.");
        }

        public void RegisterAction<T>() where T : aszAction, new()
        {
            m_AIMediator.ActionService.RegisterAction<T>();
            Debug.Log("Register Action: " + typeof(T).Name);
        }

        [ContextMenu("TestPrompt")]
        public void de_TestPrompt()
        {
            TestSendPrompt("HelloWorld");
        }

        public async void TestSendPrompt(string input)
        {
            Debug.Log($"[AishizuCore] Sending test prompt: {input}");

            try
            {
                // Send a message to the locally running LM Studio server
                string response = await m_AIMediator.SendPromptAsync(input);

                if (string.IsNullOrEmpty(response))
                    Debug.LogWarning("[AishizuCore] Empty response from LM Studio.");
                else
                    Debug.Log($"[AishizuCore] LLM Response:\n{response}");
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"[AishizuCore] HTTP request failed: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AishizuCore] Unexpected error: {e}");
            }
        }

        public async void SendPrompt(string input)
        {
            string response = await m_AIMediator.SendPromptAsync(input);
            Debug.Log($"[AishizuCore] LLM Response: {response}");
        }
    }
}
