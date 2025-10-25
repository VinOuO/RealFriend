using Aishizu.UnityCore;
using Aishizu.VRMBridge.Actions;
using UnityEngine;

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
            m_Theater.StartAct();
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
