using UnityEngine;
using Aishizu.Native.Services;
using Aishizu.Native.Actions;
using System.Collections;
using Aishizu.Native;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Aishizu.UnityCore
{
    [RequireComponent(typeof(aszActorManager))]
    [RequireComponent(typeof(aszInterableManager))]
    public class aszTheater : MonoBehaviour
    {
        public List<aszAction> de_Sequence;

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

        public void RegisterAction<T>() where T : aszAction, new()
        {
            m_AIMediator.ActionService.RegisterAction<T>();
            Debug.Log("[AishizuCore] Register Action: " + typeof(T).Name);
        }
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
            m_AIMediator.CurrentSequence.Start();
            while (!m_AIMediator.CurrentSequence.IsFinished)
            {
                m_AIMediator.CurrentSequence.Tick(Time.deltaTime);
                yield return aszUnityCoroutine.WaitForEndOfFrame;
            }
        }
    }
}
