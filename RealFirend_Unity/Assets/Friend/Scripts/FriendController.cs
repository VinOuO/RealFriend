using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;

public class FriendController : MonoBehaviour
{
    /// <summary>
    /// The usual moving speed of Friend
    /// </summary>
    [Header("Settings")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float NormalMovingSpeed = 1.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float FinalMovingSpeed = 0.0f;
    [SerializeField] AnimationCurve MovingSpeedTowardTargetCurve;

    /// <summary>
    /// How fast does it take for friend to reach an object
    /// </summary>
    [Range(0.1f, 5.0f)]
    [SerializeField] float ReachingDuraction = 0.1f;
    [SerializeField] AnimationCurve ReachingTowardTargetCurve;


    [Header("Configeration")]
    [SerializeField] VRMAnimationController m_VRMAnimationController;
    [SerializeField] FriendBodyInfo m_FriendBodyInfo;
    [SerializeField] FullBodyBipedIK m_FullBodyBipedIK;

    [Header("Debug")]
    [SerializeField] GameObject de_TouchingTarget;

    void OnEnable()
    {
        m_FriendBodyInfo = GetComponent<FriendBodyInfo>();
        m_FullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
    }

    void Update()
    {
        m_VRMAnimationController.TravelingSpeed = FinalMovingSpeed;
    }

    [ContextMenu("TouchObject")]
    public void de_TouchObject()
    {
        TouchObject(GameObject.Find("Dummy").GetComponentInChildren<BodyInfo>().GetSupportJoints.LeftCheek.gameObject);
        //TouchObject(de_TouchingTarget);
    }

    public void TouchObject(GameObject obj)
    {
        StartCoroutine(TouchingObject(obj, HumanBodyBones.LeftHand));
    }

    [ContextMenu("WalkToTarget")]
    public void WalkToTarget()
    {
        StartCoroutine(WalkingToPosition(Vector3.zero, 0.5f));
    }

    IEnumerator TouchingObject(GameObject obj, HumanBodyBones usingJoint)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        yield return StartCoroutine(WalkingToObjectPosition(obj.transform, (usingJoint != HumanBodyBones.RightHand ? m_FriendBodyInfo.GetLimbLength.LeftArmLength : m_FriendBodyInfo.GetLimbLength.RightArmLength) * 0.8f));
        float startReachingTime = Time.time;

        IKEffector effector = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftHandEffector : m_FullBodyBipedIK.solver.rightHandEffector;
        effector.target = obj.transform;
        while ((Time.time - startReachingTime) / ReachingDuraction < 1)
        {
            effector.positionWeight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime)/ReachingDuraction);
            yield return wait;
        }
    }

    IEnumerator WalkingToObjectPosition(Transform obj, float distanceThreshold)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float distance = Vector3.Distance(obj.position.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));

        while (distance > distanceThreshold)
        {
            transform.LookAt(obj.position.ToLevelPosition(transform), Vector3.up);
            FinalMovingSpeed = NormalMovingSpeed * MovingSpeedTowardTargetCurve.Evaluate(distance - (distanceThreshold * 0.75f));
            transform.Translate((obj.position.ToLevelPosition(transform) - transform.position.ToLevelPosition(transform)).normalized * Time.deltaTime * FinalMovingSpeed);
            yield return wait;
            distance = Vector3.Distance(obj.position.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));
        }
        FinalMovingSpeed = 0;
    }

    IEnumerator WalkingToPosition(Vector3 pos, float distanceThreshold)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float distance = Vector3.Distance(pos.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));
        
        while (distance > distanceThreshold)
        {
            transform.LookAt(pos.ToLevelPosition(transform), Vector3.up);
            FinalMovingSpeed = NormalMovingSpeed * MovingSpeedTowardTargetCurve.Evaluate(distance - (distanceThreshold * 0.75f));
            transform.Translate((pos.ToLevelPosition(transform) - transform.position.ToLevelPosition(transform)).normalized * Time.deltaTime * FinalMovingSpeed);
            yield return wait;
            distance = Vector3.Distance(pos.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));
        }
        FinalMovingSpeed = 0;
    }
}
