using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine.InputSystem.HID;

public class FriendController : MonoBehaviour
{
    /// <summary>
    /// The usual moving speed of friend
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

    /// <summary>
    /// How fast does it take for friend to kiss an object
    /// </summary>
    [Range(0.1f, 5.0f)]
    [SerializeField] float KissingDuraction = 0.1f;
    [SerializeField] AnimationCurve KissingTowardTargetCurve;

    [Header("Configeration")]
    [SerializeField] VRMAnimationController m_VRMAnimationController;
    [SerializeField] FriendBodyInfo m_FriendBodyInfo;
    [SerializeField] FullBodyBipedIK m_FullBodyBipedIK;
    [SerializeField] FBBIKHeadEffector m_FBBIKHeadEffector;
    [SerializeField] BodyStatus m_BodyStatus;
    [Header("AutoConfigeration")]
    [SerializeField] Transform m_LeftHandTarget;
    [SerializeField] Transform m_RightHandTarget;
    [SerializeField] Transform m_LeftFootTarget;
    [SerializeField] Transform m_RightFootTarget;
    [Header("Debug")]
    [SerializeField] GameObject de_TouchingTarget;
    [SerializeField] Sitable de_SittingTarget;

    void OnEnable()
    {
        m_FriendBodyInfo = GetComponent<FriendBodyInfo>();
        m_FullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
        m_FBBIKHeadEffector = GetComponentInChildren<FBBIKHeadEffector>();
        m_BodyStatus = GetComponentInChildren<BodyStatus>();
        Init();
    }

    private void Init()
    {
        m_LeftHandTarget = new GameObject(name + "_LeftHandTarget").transform;
        m_RightHandTarget = new GameObject(name + "_RightHandTarget").transform;
        m_LeftFootTarget = new GameObject(name + "_LeftFootTarget").transform;
        m_RightFootTarget = new GameObject(name + "_RightFootTarget").transform;
        m_FullBodyBipedIK.solver.leftHandEffector.target = m_LeftHandTarget;
        m_FullBodyBipedIK.solver.rightHandEffector.target = m_RightHandTarget;
        m_FullBodyBipedIK.solver.leftFootEffector.target = m_LeftFootTarget;
        m_FullBodyBipedIK.solver.rightFootEffector.target = m_RightFootTarget;
        m_FullBodyBipedIK.solver.leftHandEffector.VRMSetTarget(m_FriendBodyInfo.GetHumanoid.LeftHand);
        m_FullBodyBipedIK.solver.rightHandEffector.VRMSetTarget(m_FriendBodyInfo.GetHumanoid.RightHand);
        m_FullBodyBipedIK.solver.leftFootEffector.VRMSetTarget(m_FriendBodyInfo.GetHumanoid.LeftFoot);
        m_FullBodyBipedIK.solver.rightFootEffector.VRMSetTarget(m_FriendBodyInfo.GetHumanoid.RightFoot);
        StartCoroutine(TippingToes());
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
    #region Reach
    [ContextMenu("ReachObject")]
    public void de_ReachObject()
    {
        ReachObject(GameObject.Find("Dummy").GetComponentInChildren<BodyInfo>().GetSupportJoints.LeftCheek.gameObject);
    }
    public void ReachObject(GameObject obj)
    {
        StartCoroutine(ReachingObject(obj, HumanBodyBones.LeftHand));
    }
    IEnumerator ReachingObject(GameObject obj, HumanBodyBones usingJoint)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        float startReachingTime = Time.time;
        IKEffector effector = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftHandEffector : m_FullBodyBipedIK.solver.rightHandEffector;
        IKMappingLimb mapping = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftArmMapping : m_FullBodyBipedIK.solver.rightArmMapping;
        effector.VRMSetTarget(obj.transform, m_FullBodyBipedIK);
        while ((Time.time - startReachingTime) / ReachingDuraction < 1)
        {
            mapping.weight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
            effector.positionWeight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
            effector.rotationWeight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
            yield return wait;
        }
    }
    #endregion
    #region Touch
    [ContextMenu("TouchObject")]
    public void de_TouchObject()
    {
        TouchObject(GameObject.Find("Dummy").GetComponentInChildren<BodyInfo>().GetSupportJoints.LeftCheek.gameObject);
    }
    public void TouchObject(GameObject obj)
    {
        StartCoroutine(TouchingObject(obj, HumanBodyBones.LeftHand));
    }
    IEnumerator TouchingObject(GameObject obj, HumanBodyBones usingJoint)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        yield return StartCoroutine(WalkingToObjectPosition(obj.transform, (usingJoint != HumanBodyBones.RightHand ? m_FriendBodyInfo.GetBodyCode.LeftArmLength : m_FriendBodyInfo.GetBodyCode.RightArmLength) * 0.8f));
        yield return StartCoroutine(ReachingObject(obj, usingJoint));
    }
    #endregion
    #region Sit
    [ContextMenu("SitOnTarget")]
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
    #region Kiss
    [ContextMenu("KissObject")]
    public void de_KissObject()
    {
        KissObject(GameObject.Find("Dummy").GetComponentInChildren<BodyInfo>().GetSupportJoints.HeadCenter.gameObject, GameObject.Find("Dummy").GetComponentInChildren<Holdable>());
    }
    public void KissObject(GameObject kissObj, Holdable holdObj)
    {
        StartCoroutine(KissingObjectWhileHolding(kissObj, holdObj));
    }
    IEnumerator KissingObject(GameObject kissObj)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        EnableTippingToes(true);
        m_FBBIKHeadEffector.positionWeight = 0;
        m_FBBIKHeadEffector.VRMSetTarget(kissObj.transform);
        yield return StartCoroutine(WalkingToObjectPosition(kissObj.transform, m_FriendBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
        float startKissingTime = Time.time;
        while ((Time.time - startKissingTime) / KissingDuraction < 0.8f && m_BodyStatus.IsGrounded(BodyStatus.GroundedPart.Tipping))
        {
            m_FBBIKHeadEffector.positionWeight = KissingTowardTargetCurve.Evaluate((Time.time - startKissingTime) / KissingDuraction);
            yield return wait;
        }
    }

    IEnumerator KissingObjectWhileHolding(GameObject kissObj, Holdable holdObj)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        if (m_BodyStatus.HoldingObj != holdObj)
        {
            yield return StartCoroutine(HoldingObject(holdObj));
        }
        StartCoroutine(KissingObject(kissObj));
    }
    #endregion
    #region Hold
    [ContextMenu("HoldObject")]
    public void de_HoldObject()
    {
        HoldObject(GameObject.Find("Dummy").GetComponentInChildren<Holdable>());
    }
    public void HoldObject(Holdable holdObj)
    {
        StartCoroutine(HoldingObject(holdObj));
    }
    IEnumerator HoldingObject(Holdable holdObj)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        yield return StartCoroutine(WalkingToObjectPosition(holdObj.transform, m_FriendBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
        bool leftHandReached = false, rightHandReached = false;
        StartCoroutine(CoroutineWithCallback(ReachingObject(holdObj.HoldTrans[0].gameObject, HumanBodyBones.LeftHand), () => leftHandReached = true));
        StartCoroutine(CoroutineWithCallback(ReachingObject(holdObj.HoldTrans[1].gameObject, HumanBodyBones.RightHand), () => rightHandReached = true));
        yield return new WaitUntil(() => leftHandReached && rightHandReached);
        m_BodyStatus.HoldingObj = holdObj;
    }
    #endregion
    #region Hug
    [ContextMenu("HugObject")]
    public void de_HugObject()
    {
        HugObject(GameObject.Find("Dummy").GetComponentInChildren<Hugable>());
    }
    public void HugObject(Hugable hugObj)
    {
        StartCoroutine(HugingObject(hugObj));
    }
    IEnumerator HugingObject(Hugable hugObj)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        yield return StartCoroutine(WalkingToObjectPosition(hugObj.transform, hugObj.HugableDistance));
        m_VRMAnimationController.PlayHug();
        m_BodyStatus.HugingObj = hugObj;
    }
    #endregion
    #region TipToes
    private bool EnableTipping = false;
    private void EnableTippingToes(bool enable)
    {
        if (enable)
        {
            m_FullBodyBipedIK.solver.leftFootEffector.rotationWeight = 1f;
            m_FullBodyBipedIK.solver.rightFootEffector.rotationWeight = 1f;
            m_LeftFootTarget.localRotation = Quaternion.identity;
            m_RightFootTarget.localRotation = Quaternion.identity;
        }
        else
        {
            m_FullBodyBipedIK.solver.leftFootEffector.rotationWeight = 0f;
            m_FullBodyBipedIK.solver.rightFootEffector.rotationWeight = 0f;
        }
        EnableTipping = enable;
    }
    IEnumerator TippingToes()
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        while (true)
        {
            if (EnableTipping)
            {
                if (!m_BodyStatus.IsGrounded(BodyStatus.GroundedPart.LeftToes))
                {
                    Vector3 leftFootAngle = m_FriendBodyInfo.GetHumanoid.LeftFoot.eulerAngles;
                    if (leftFootAngle.x > 180)
                    {
                        leftFootAngle.x -= 360;
                    }
                    if (leftFootAngle.x < 35f)
                    {
                        m_LeftFootTarget.localRotation *= Quaternion.AngleAxis(Time.deltaTime * 200f, m_FriendBodyInfo.GetHumanoid.LeftFoot.localRotation * Vector3.right);
                    }
                }

                if (!m_BodyStatus.IsGrounded(BodyStatus.GroundedPart.RightToes))
                {
                    Vector3 rightFootAngle = m_FriendBodyInfo.GetHumanoid.RightFoot.localRotation.eulerAngles;
                    if (rightFootAngle.x > 180)
                    {
                        rightFootAngle.x -= 360;
                    }
                    if (rightFootAngle.x < 35f)
                    {
                        m_RightFootTarget.localRotation *= Quaternion.AngleAxis(Time.deltaTime * 200f, m_FriendBodyInfo.GetHumanoid.RightFoot.localRotation * Vector3.right);
                    }
                }
            }
            yield return wait;
        }
    }
    #endregion









    public IEnumerator CoroutineWithCallback(IEnumerator targetCoroutine, Action onComplete)
    {
        yield return StartCoroutine(targetCoroutine);
        onComplete?.Invoke();
    }
}
