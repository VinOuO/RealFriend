using UnityEngine;

namespace Aishizu.UnityCore
{
    public abstract class aszInteractable : MonoBehaviour
    {
        [SerializeField] private Transform m_InteractTransform; public Transform InteractTransform => m_InteractTransform;
        private void OnEnable()
        {
            if(m_InteractTransform == null)
            {
                m_InteractTransform = transform;
            }
        }
    }
}


