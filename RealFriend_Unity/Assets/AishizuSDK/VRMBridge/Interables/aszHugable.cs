using Aishizu.UnityCore;
using UnityEngine;

namespace Aishizu.VRMBridge
{
    public class aszHugable : aszInteractable
    {
        [SerializeField] private float m_HugableDistance = 0.1f; public float HugableDistance => m_HugableDistance;
    }

}
