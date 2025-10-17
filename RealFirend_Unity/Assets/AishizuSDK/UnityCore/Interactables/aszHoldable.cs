using UnityEngine;
public class aszHoldable : aszInteractable
{
   [SerializeField] private Transform[] m_holdTrans; public Transform[] HoldTrans => m_holdTrans;
}
