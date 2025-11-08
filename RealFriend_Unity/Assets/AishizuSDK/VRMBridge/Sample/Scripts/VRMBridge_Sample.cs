using Aishizu.UnityCore;
using Aishizu.VRMBridge.Actions;
using UnityEngine;
using System.Collections;
using Aishizu.Native;
using System.Threading.Tasks;

namespace Aishizu.VRMBridge
{
    public class VRMBridge_Sample : MonoBehaviour
    {
        [SerializeField] private aszScriptManager m_ScriptManager;
        [SerializeField] private aszVRMBodyInfo m_DummyBodyInfo;
        private void Start()
        {
            RegisterActions();
            AddInterables();
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Task<Result> setupTask = m_ScriptManager.SetUpStage();
            yield return new WaitUntil(() => setupTask.IsCompleted);
            Result result = setupTask.Result;
            if (setupTask.Result != Result.Success)
            {
                Debug.Log("Fail SetUpStage");
                yield break;
            }

            Task<Result> updateTask = m_ScriptManager.UpdateStage();
            yield return new WaitUntil(() => updateTask.IsCompleted);
            if (updateTask.Result != Result.Success)
            {
                Debug.Log("Fail UpdateStage");
                yield break;
            }
            yield return StartCoroutine(aszVRMScriptDirector.Instance.RunningScript(m_ScriptManager.GetScript())) ;
        }

        private void AddInterables()
        {
            if(m_DummyBodyInfo.GetSupportJoints.Mouth.GetComponent<aszKissable>() == null)
            {
                return;
            }
            m_ScriptManager.InterableManager.AddInterable(m_DummyBodyInfo.GetSupportJoints.Mouth.GetComponent<aszKissable>());
            m_ScriptManager.InterableManager.AddInterable(m_DummyBodyInfo.GetSupportJoints.HeadCenter.GetComponent<aszHoldable>());
        }

        [ContextMenu("RegisterActions")]
        private void RegisterActions()
        {
            Debug.Log(m_ScriptManager == null ? "null" : "not null");
            m_ScriptManager.RegisterAction<aszVRMHold>();
            m_ScriptManager.RegisterAction<aszVRMHug>();
            m_ScriptManager.RegisterAction<aszVRMKiss>();
            m_ScriptManager.RegisterAction<aszVRMReach>();
            m_ScriptManager.RegisterAction<aszVRMSit>();
            m_ScriptManager.RegisterAction<aszVRMTouch>();
            m_ScriptManager.RegisterAction<aszVRMWalk>();
        }
    }
}
