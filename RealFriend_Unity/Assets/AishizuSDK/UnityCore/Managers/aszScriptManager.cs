using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.Native;
using System.Threading.Tasks;

namespace Aishizu.UnityCore
{
    [RequireComponent(typeof(aszActorManager))]
    [RequireComponent(typeof(aszInterableManager))]
    public class aszScriptManager : MonoBehaviour
    {
        /*
        [Header("DeveloperSettings")]
        [Tooltip("The events will be trigger when an aszAction is started.\r\nThis can be used to call the actual performance and init functions of your action.")]
        [SerializeField] UnityEvent<aszAction> OnActionStart;
        [Tooltip("The events will be trigger when an aszAction is updated.")]
        [SerializeField] UnityEvent<aszAction> OnActionUpdate;
        [Tooltip("The events will be trigger when an aszAction is finished.\r\nThis can be useful when you wish to clean up the performance of an action.")]
        [SerializeField] UnityEvent<aszAction> OnActionEnd;
        [Tooltip("The events will be trigger when the enmotion of any character is changed.")]
        [SerializeField] UnityEvent<aszEmotionChange> OnEnmotionChange;
        */

        #region Init
        public static aszScriptManager Instance;
        private aszScriptWriter m_Writter; 
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
            InitializeScriptWriter();
        }

        private void InitializeScriptWriter()
        {
            m_Writter = new aszScriptWriter();
            m_Writter.Endpoint = "http://localhost:1234/v1/chat/completions";
            m_Writter.Model = "Mythomax L2 13B";
            #region Register Actors
            for (int i = 0; i < m_ActorManager.ActorList.Count; i++)
            {
                m_Writter.ActorService.RegisterActor(i, m_ActorManager.ActorList[i].name);
            }
            #endregion
            #region Register Interables
            for (int i = 0; i < m_InterableManager.InterableList.Count; i++)
            {
                m_Writter.TargetService.RegisterTarget(i, m_InterableManager.InterableList[i].name);
            }
            #endregion

            Debug.Log("[AishizuCore] Mediator and services initialized.");
        }
        #endregion
        #region Registeration
        #region ActionType
        public void RegisterAction<T>() where T : aszAction, new()
        {
            m_Writter.ActionService.RegisterAction<T>();
            Debug.Log("[AishizuCore] Register Action: " + typeof(T).Name);
        }
        #endregion
        /*
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
        */
        #endregion
        public async Task<Result> SetUpStage()
        {
            return await m_Writter.SetUpScene();
        }
        public async Task<Result> UpdateStage(string userPrompt)
        {
            return await m_Writter.DescribeCurrentScene(userPrompt: userPrompt);
        }

        public aszScript GetScript()
        {
            return m_Writter.GetScript();
        }
    }
}
