using UnityEngine;

public class Hugable : Interactable
{
    [SerializeField] private float m_HugableDistance = 0.1f; public float HugableDistance => m_HugableDistance;
}
