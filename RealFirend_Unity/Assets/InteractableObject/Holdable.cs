using UnityEngine;
public class Holdable : Interactable
{
   [SerializeField] private Transform[] m_holdTrans; public Transform[] HoldTrans => m_holdTrans;
}
