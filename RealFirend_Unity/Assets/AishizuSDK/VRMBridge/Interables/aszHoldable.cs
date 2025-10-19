using Aishizu.UnityCore;
using UnityEngine;

namespace Aishizu.VRMBridge
{
    public class aszHoldable : aszInteractable
    {
        [SerializeField] private Transform[] m_holdTrans; public Transform[] HoldTrans => m_holdTrans;
    }

}
