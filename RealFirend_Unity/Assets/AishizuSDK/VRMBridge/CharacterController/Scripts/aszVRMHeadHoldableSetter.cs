using UnityEngine;

namespace Aishizu.VRMBridge
{
    public class aszVRMHeadHoldableSetter : MonoBehaviour
    {
        [SerializeField] aszVRMBodyInfo m_BodyInfo;
        [SerializeField] aszHoldable m_Holdable;
        void Start()
        {
            m_Holdable.HoldTrans[0] = m_BodyInfo.GetSupportJoints.LeftCheek;
            m_Holdable.HoldTrans[1] = m_BodyInfo.GetSupportJoints.RightCheek;
        }
    }
}
