using Aishizu.UnityCore;
using Aishizu.VRMBridge.Actions;
using UnityEngine;
using System.Collections;
using Aishizu.Native;
using System.Threading.Tasks;

namespace Aishizu.VRMBridge
{
    public class TestTheater : MonoBehaviour
    {
        [SerializeField] private aszTheater m_Theater;
        [SerializeField] private aszVRMBodyInfo m_DummyBodyInfo;
        private void Start()
        {
            RegisterActions();
            AddInterables();
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Task<Result> setupTask = m_Theater.SetUpStage();
            yield return new WaitUntil(() => setupTask.IsCompleted);
            Debug.Log("SetUp: " + setupTask.Result);
            Result result = setupTask.Result;
            if (setupTask.Result != Result.Success)
            {
                yield break;
            }

            Task<Result> updateTask = m_Theater.UpdateStage();
            yield return new WaitUntil(() => updateTask.IsCompleted);
            Debug.Log("Update: " + updateTask.Result);
            if (updateTask.Result != Result.Success)
            {
                yield break;
            }
            yield return m_Theater.PlayingTimeline();
        }

        private void AddInterables()
        {
            if(m_DummyBodyInfo.GetSupportJoints.Mouth.GetComponent<aszKissable>() == null)
            {
                Debug.Log("PPAP");
                return;
            }
            m_Theater.InterableManager.AddInterable(m_DummyBodyInfo.GetSupportJoints.Mouth.GetComponent<aszKissable>());
            m_Theater.InterableManager.AddInterable(m_DummyBodyInfo.GetSupportJoints.HeadCenter.GetComponent<aszHoldable>());
        }

        [ContextMenu("RegisterActions")]
        private void RegisterActions()
        {
            m_Theater.RegisterAction<aszVRMHold>();
            m_Theater.RegisterAction<aszVRMHug>();
            m_Theater.RegisterAction<aszVRMKiss>();
            m_Theater.RegisterAction<aszVRMReach>();
            m_Theater.RegisterAction<aszVRMSit>();
            m_Theater.RegisterAction<aszVRMTouch>();
            m_Theater.RegisterAction<aszVRMWalk>();
        }
    }
}
