using UnityEngine;
using Aishizu.Native.Services;
using Aishizu.Native.Actions;
using System.Collections;
using Aishizu.Native;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Aishizu.Native.Events;

namespace Aishizu.UnityCore
{
    [RequireComponent(typeof(aszActorManager))]
    [RequireComponent(typeof(aszInterableManager))]
    public class aszTheater : MonoBehaviour
    {
        [Header("DeveloperSettings")]
        [Tooltip("The events will be trigger when an aszAction is started.\r\nThis can be used to call the actual performance and init functions of your action.")]
        [SerializeField] UnityEvent<aszAction> OnActionStart;
        [Tooltip("The events will be trigger when an aszAction is updated.")]
        [SerializeField] UnityEvent<aszAction> OnActionUpdate;
        [Tooltip("The events will be trigger when an aszAction is finished.\r\nThis can be useful when you wish to clean up the performance of an action.")]
        [SerializeField] UnityEvent<aszAction> OnActionEnd;
        [Tooltip("The events will be trigger when the enmotion of any character is changed.")]
        [SerializeField] UnityEvent<aszEmotionChange> OnEnmotionChange;

        #region Init
        public static aszTheater Instance;
        private aszAIMediator m_AIMediator; 
        private aszActorManager m_ActorManager; public aszActorManager ActorManager => m_ActorManager;
        private aszInterableManager m_InterableManager; public aszInterableManager InterableManager => m_InterableManager;


        private void OnEnable()
        {
            Init();
        }

        [ContextMenu("Init")]
        private void Init()
        {
            Instance = this;
            m_ActorManager = GetComponent<aszActorManager>();
            m_InterableManager = GetComponent<aszInterableManager>();
            InitializeMediator();
            m_AIMediator.SequenceService.OnActionStart += InvockUnityEvent_OnActionStart;
            m_AIMediator.SequenceService.OnActionUpdate += InvockUnityEvent_OnActionUpdate;
            m_AIMediator.SequenceService.OnActionEnd += InvockUnityEvent_OnActionEnd;
            m_AIMediator.SequenceService.OnEmotionChange += InvockUnityEvent_OnEmotionChange;
        }

        private void InitializeMediator()
        {
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
        #endregion
        #region Registeration
        #region ActionType
        public void RegisterAction<T>() where T : aszAction, new()
        {
            m_AIMediator.ActionService.RegisterAction<T>();
            Debug.Log("[AishizuCore] Register Action: " + typeof(T).Name);
        }
        #endregion
        #region Unity Action Event
        public void InvockUnityEvent_OnActionStart(aszAction action)
        {
            OnActionStart.Invoke(action);
        }
        public void InvockUnityEvent_OnActionUpdate(aszAction action, float deltaTime)
        {
            OnActionUpdate.Invoke(action);
        }
        public void InvockUnityEvent_OnActionEnd(aszAction action)
        {
            OnActionEnd.Invoke(action);
        }
        public void InvockUnityEvent_OnEmotionChange(aszEmotionChange emotionChange)
        {
            OnEnmotionChange.Invoke(emotionChange);
        }
        #endregion
        #endregion
        public async Task<Result> SetUpStage()
        {
            return await m_AIMediator.SetUpScene();
        }

        public async Task<Result> UpdateStage()
        {
            return await m_AIMediator.DescribeCurrentScene();
        }

        public IEnumerator PlayingTimeline()
        {
            Debug.Log(@"[aszTheater] Start Sequence");
            m_AIMediator.SequenceService.Start();
            while (!m_AIMediator.SequenceService.IsFinished)
            {
                m_AIMediator.SequenceService.Tick(Time.deltaTime);
                yield return aszUnityCoroutine.WaitForEndOfFrame;
            }
        }
    }
}
