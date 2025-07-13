using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendController : MonoBehaviour
{
    /// <summary>
    /// The usual moving speed of Friend
    /// </summary>
    [Header("Settings")]
    [Range(0.0f, 1.0f)]
    public float NormalMovingSpeed = 1.0f;
    [Range(0.0f, 1.0f)]
    public float FinalMovingSpeed = 0.0f;
    public AnimationCurve MovingSpeedTowardTargetCurve;

    [Header("Configeration")]
    [SerializeField] VRMAnimationController m_VRMAnimationController;
    
    void Start()
    {
        
    }

    void Update()
    {
        m_VRMAnimationController.TravelingSpeed = FinalMovingSpeed;
    }

    [ContextMenu("WalkToTarget")]
    public void WalkToTarget()
    {
        StartCoroutine(WalkingTowardTarget(Vector3.zero));
    }

    IEnumerator WalkingTowardTarget(Vector3 target)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float distance = Vector2.Distance(target.ToLevelPosition(), transform.position.ToLevelPosition());
        
        while (distance > 0.5f)
        {
            transform.LookAt(target, Vector3.up);
            FinalMovingSpeed = NormalMovingSpeed * MovingSpeedTowardTargetCurve.Evaluate(distance - 0.35f);
            transform.Translate((target - transform.position).normalized * Time.deltaTime * FinalMovingSpeed);
            yield return wait;
            distance = Vector2.Distance(target.ToLevelPosition(), transform.position.ToLevelPosition());
        }


        FinalMovingSpeed = 0;
    }
}
