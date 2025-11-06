using Aishizu.UnityCore;
using UnityEngine;

namespace Aishizu.VRMBridge
{
    public class aszSitable : aszInteractable
    {
        [SerializeField] private float m_Edge; public float Edge => m_Edge;
        [SerializeField] private Transform m_SitPose; public Transform SitPose => m_SitPose;
    }

}
