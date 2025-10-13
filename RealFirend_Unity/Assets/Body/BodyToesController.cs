using UnityEngine;

public class BodyToesController : MonoBehaviour
{
    [SerializeField] bool m_IsGounded = true; public bool IsGounded => m_IsGounded;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_IsGounded = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_IsGounded = true;
        }
    }
}
