using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using Unity.VisualScripting;

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
    [SerializeField] AnimationCurve FacingTargetCurve;
    [SerializeField] AnimationCurve SittingElevateCurve;

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
    [SerializeField] Sitable de_SittingTarget;

    void OnEnable()
    {
        m_FriendBodyInfo = GetComponent<FriendBodyInfo>();
        m_FullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
    }

    void Update()
    {
        m_VRMAnimationController.TravelingSpeed = FinalMovingSpeed;
    }
    #region Walk
    [ContextMenu("WalkToTarget")]
    public void WalkToTarget()
    {
        StartCoroutine(WalkingToPosition(Vector3.zero, 0.5f));
    }

    IEnumerator WalkingToObjectPosition(Transform obj, float distanceThreshold)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float distance = Vector3.Distance(obj.position.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));

        while (distance > distanceThreshold)
        {
            WalkToPos(obj.position, distanceThreshold, ref distance);
            yield return wait;

        }
        FinalMovingSpeed = 0;
    }
    IEnumerator WalkingToPosition(Vector3 pos, float distanceThreshold)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float distance = Vector3.Distance(pos.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));

        while (distance > distanceThreshold)
        {
            WalkToPos(pos, distanceThreshold, ref distance);
            yield return wait;
        }
        FinalMovingSpeed = 0;
    }
    private void WalkToPos(Vector3 pos, float distanceThreshold, ref float distance)
    {
        transform.LookAt(pos.ToLevelPosition(transform), Vector3.up);
        FinalMovingSpeed = NormalMovingSpeed * MovingSpeedTowardTargetCurve.Evaluate(distance - (distanceThreshold * 0.75f));
        transform.Translate((pos.ToLevelPosition(transform) - transform.position.ToLevelPosition(transform)).normalized * Time.deltaTime * FinalMovingSpeed);
        distance = Vector3.Distance(pos.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));
    }
    #endregion
    #region Facing
    private IEnumerator Facing(Vector3 pos, float withInDuraction)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float angleDiff = Vector3.SignedAngle(transform.forward, (pos - transform.position).normalized, Vector3.up);
        float startFacingTime = Time.time;
        Quaternion originalFacing = transform.rotation;
        while (Time.time - startFacingTime < withInDuraction)
        {
            float currentAngle = angleDiff * FacingTargetCurve.Evaluate((Time.time - startFacingTime) / withInDuraction);
            transform.rotation = originalFacing * Quaternion.AngleAxis(currentAngle, Vector3.up); ;
            yield return wait;
        }
        transform.LookAt(pos.ToLevelPosition(transform), Vector3.up);
    }
    #endregion
    #region Touch
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
    IEnumerator TouchingObject(GameObject obj, HumanBodyBones usingJoint)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        yield return StartCoroutine(WalkingToObjectPosition(obj.transform, (usingJoint != HumanBodyBones.RightHand ? m_FriendBodyInfo.GetBodyCode.LeftArmLength : m_FriendBodyInfo.GetBodyCode.RightArmLength) * 0.8f));
        float startReachingTime = Time.time;

        IKEffector effector = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftHandEffector : m_FullBodyBipedIK.solver.rightHandEffector;
        IKMappingLimb mapping = m_FullBodyBipedIK.solver.leftArmMapping;
        effector.target = obj.transform;
        while ((Time.time - startReachingTime) / ReachingDuraction < 1)
        {
            mapping.weight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
            effector.positionWeight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
            effector.rotationWeight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
            yield return wait;
        }
    }
    #endregion
    #region Sit
    [ContextMenu("SitOnTarget")]
    [ContextMenu("TouchObject")]
    public void de_SitOnObject()
    {
        SitOnObject(de_SittingTarget);
        //TouchObject(de_TouchingTarget);
    }
    public void SitOnObject(Sitable sitObj)
    {
        StartCoroutine(SittingOnObject(sitObj));
    }

    private IEnumerator SitingElevatingToHeight(float targetHeight, float withInDuraction, Transform hip)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float startElevatingTime = Time.time;
        Vector3 originalPos = transform.position;
        while ((Time.time - startElevatingTime) < withInDuraction)
        {
            transform.position = transform.position + Vector3.up * (targetHeight - hip.position.y) * SittingElevateCurve.Evaluate((Time.time - startElevatingTime) / withInDuraction);
            yield return wait;
        }
        transform.position = transform.position + Vector3.up * (targetHeight - hip.position.y);
    }

    private IEnumerator SittingOnObject(Sitable sitObj)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        yield return StartCoroutine(WalkingToObjectPosition(sitObj.transform, 0.05f));
        yield return StartCoroutine(Facing(sitObj.transform.position + sitObj.transform.forward, 1f));
        m_VRMAnimationController.GetAnimator.SetTrigger("TriggerSit");
        yield return StartCoroutine(SitingElevatingToHeight(sitObj.SitPose.position.y + m_FriendBodyInfo.GetBodyCode.HipRadious, 3f, m_FriendBodyInfo.GetHumanoid.GetBoneTransform(HumanBodyBones.Spine)));
    }
    #endregion
}
