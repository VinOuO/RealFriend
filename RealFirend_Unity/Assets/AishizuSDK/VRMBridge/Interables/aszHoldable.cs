using Aishizu.UnityCore;
using UnityEngine;

namespace Aishizu.VRMBridge
{
    public class aszHoldable : aszInteractable
    {
        [SerializeField] private Transform[] m_holdTrans = new Transform[2]; public Transform[] HoldTrans { get { return m_holdTrans; } set { m_holdTrans = value; } }
    }

}
