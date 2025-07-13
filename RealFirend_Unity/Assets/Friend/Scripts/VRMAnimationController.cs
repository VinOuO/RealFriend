using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRMAnimationController : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    public float TravelingSpeed = 0;

    void Start()
    {
        //StartCoroutine(CaulatingMovingSpeed());
    }

    void Update()
    {
        float lerpedTravelingSpeed = Mathf.InverseLerp(0, 0.2f, TravelingSpeed);
        m_Animator.SetFloat("TravelingSpeed", lerpedTravelingSpeed);
        m_Animator.SetFloat("MovementAnimSpeed", Mathf.Clamp(lerpedTravelingSpeed, 0.1f, 1f));
    }

    IEnumerator CaulatingMovingSpeed()
    {
        YieldInstruction wait = new WaitForFixedUpdate();
        Vector2 oldPos2D, newPos2D;
        oldPos2D = transform.position.ToLevelPosition();
        while (true)
        {
            newPos2D = transform.position.ToLevelPosition();
            TravelingSpeed = Vector2.Distance(newPos2D, oldPos2D) / Time.fixedDeltaTime;
            oldPos2D = newPos2D;
            yield return wait;
        }
    }
}
