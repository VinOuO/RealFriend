using UnityEngine;
using System.Collections;
using System;
using RootMotion.FinalIK;
using Aishizu.UnityCore;
using Aishizu.UnityCore.Actions;

namespace Aishizu.VRMBridge
{
    public class aszVRMCharacterController : aszCharacterController
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
        [SerializeField] aszVRMAnimationController m_VRMAnimationController;
        [SerializeField] aszVRMBodyInfo m_VRMBodyInfo;
        [SerializeField] FullBodyBipedIK m_FullBodyBipedIK;
        [SerializeField] FBBIKHeadEffector m_FBBIKHeadEffector;
        [SerializeField] FriendSpeachController m_FriendSpeachController;
        [SerializeField] aszVRMBodyStatus m_BodyStatus;
        [Header("AutoConfigeration")]
        [SerializeField] Transform m_LeftHandTarget;
        [SerializeField] Transform m_RightHandTarget;
        [SerializeField] Transform m_LeftFootTarget;
        [SerializeField] Transform m_RightFootTarget;
        [Header("Debug")]
        [SerializeField] GameObject de_TouchingTarget;
        [SerializeField] aszSitable de_SittingTarget;

        void OnEnable()
        {
            m_VRMBodyInfo = GetComponent<aszVRMBodyInfo>();
            m_FullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
            m_FBBIKHeadEffector = GetComponentInChildren<FBBIKHeadEffector>();
            m_FriendSpeachController = GetComponentInChildren<FriendSpeachController>();
            m_BodyStatus = GetComponentInChildren<aszVRMBodyStatus>();
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
            m_FullBodyBipedIK.solver.leftHandEffector.VRMSetTarget(m_VRMBodyInfo.GetHumanoid.LeftHand);
            m_FullBodyBipedIK.solver.rightHandEffector.VRMSetTarget(m_VRMBodyInfo.GetHumanoid.RightHand);
            m_FullBodyBipedIK.solver.leftFootEffector.VRMSetTarget(m_VRMBodyInfo.GetHumanoid.LeftFoot);
            m_FullBodyBipedIK.solver.rightFootEffector.VRMSetTarget(m_VRMBodyInfo.GetHumanoid.RightFoot);
            StartCoroutine(TippingToes());
        }

        void Update()
        {
            m_VRMAnimationController.TravelingSpeed = FinalMovingSpeed;
        }

        public override IEnumerator Execute(aszIUnityAction action)
        {
            switch (action)
            {
                case aszUnityActionWalk walk:
                    yield return StartCoroutine(WalkingToObject(walk));
                    break;

                case aszUnityActionReach reach:
                    yield return StartCoroutine(ReachingObject(reach));
                    break;

                case aszUnityActionTouch touch:
                    yield return StartCoroutine(TouchingObject(touch));
                    break;

                case aszUnityActionHug hug:
                    yield return StartCoroutine(HugingObject(hug));
                    break;

                case aszUnityActionKiss kiss:
                    yield return StartCoroutine(KissingObject(kiss));
                    break;
                case aszUnityActionHold hold:
                    yield return StartCoroutine(HoldingObject(hold));
                    break;
                case aszUnityActionSit sit:
                    yield return StartCoroutine(SittingOnObject(sit));
                    break;

                default:
                    Debug.LogWarning($"[VRMCharacterController] Unknown action type: {action.GetType()}");
                    break;
            }
        }

        #region Walk
        [ContextMenu("WalkToTarget")]
        public void WalkToTarget()
        {
            StartCoroutine(WalkingToPosition(Vector3.zero, 0.5f));
        }

        private IEnumerator WalkingToObject(aszUnityActionWalk walk)
        {
            yield return StartCoroutine(WalkingToObject(walk.TargetObject.transform, walk.StopDistance));
        }

        private IEnumerator WalkingToObject(Transform obj, float stopDistance)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            float distance = Vector3.Distance(obj.transform.position.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));

            while (distance > stopDistance)
            {
                WalkToPos(obj.position, stopDistance, ref distance);
                yield return wait;
            }
            FinalMovingSpeed = 0;
        }
        private IEnumerator WalkingToPosition(Vector3 pos, float distanceThreshold)
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
        private IEnumerator ReachingObject(aszUnityActionReach reach)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            float startReachingTime = Time.time;
            IKEffector effector = reach.Hand != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftHandEffector : m_FullBodyBipedIK.solver.rightHandEffector;
            IKMappingLimb mapping = reach.Hand != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftArmMapping : m_FullBodyBipedIK.solver.rightArmMapping;
            effector.VRMSetTarget(reach.TargetObject.transform, m_FullBodyBipedIK);
            while ((Time.time - startReachingTime) / ReachingDuraction < 1)
            {
                mapping.weight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
                effector.positionWeight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
                effector.rotationWeight = ReachingTowardTargetCurve.Evaluate((Time.time - startReachingTime) / ReachingDuraction);
                yield return wait;
            }
        }

        private IEnumerator ReachingObject(Transform obj, HumanBodyBones usingJoint)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            float startReachingTime = Time.time;
            IKEffector effector = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftHandEffector : m_FullBodyBipedIK.solver.rightHandEffector;
            IKMappingLimb mapping = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftArmMapping : m_FullBodyBipedIK.solver.rightArmMapping;
            effector.VRMSetTarget(obj, m_FullBodyBipedIK);
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
        private IEnumerator TouchingObject(aszUnityActionTouch touch)
        {
            yield return StartCoroutine(WalkingToObject(touch.TargetObject.transform, (touch.Hand != HumanBodyBones.RightHand ? m_VRMBodyInfo.GetBodyCode.LeftArmLength : m_VRMBodyInfo.GetBodyCode.RightArmLength) * 0.8f));
            yield return StartCoroutine(ReachingObject(touch.TargetObject.transform, touch.Hand));
        }

        private IEnumerator TouchingObject(Transform obj, HumanBodyBones usingJoint)
        {
            yield return StartCoroutine(WalkingToObject(obj, (usingJoint != HumanBodyBones.RightHand ? m_VRMBodyInfo.GetBodyCode.LeftArmLength : m_VRMBodyInfo.GetBodyCode.RightArmLength) * 0.8f));
            yield return StartCoroutine(ReachingObject(obj, usingJoint));
        }
        #endregion
        #region Sit
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

        private IEnumerator SittingOnObject(aszUnityActionSit sitObj)
        {
            aszSitable sitable = sitObj.TargetObject.GetComponentInChildren<aszSitable>();
            if (sitable)
            {
                yield return StartCoroutine(SittingOnObject(sitable));
            }

        }

        private IEnumerator SittingOnObject(aszSitable sitObj)
        {
            yield return StartCoroutine(WalkingToObject(sitObj.transform, 0.05f));
            yield return StartCoroutine(Facing(sitObj.transform.position + sitObj.transform.forward, 1f));
            m_VRMAnimationController.GetAnimator.SetTrigger("TriggerSit");
            yield return StartCoroutine(SitingElevatingToHeight(sitObj.SitPose.position.y + m_VRMBodyInfo.GetBodyCode.HipRadious, 3f, m_VRMBodyInfo.GetHumanoid.GetBoneTransform(HumanBodyBones.Spine)));
        }
        #endregion
        #region Kiss
        private IEnumerator KissingObject(aszUnityActionKiss kiss)
        {
            yield return StartCoroutine(KissingObject(kiss.TargetObject.transform));
        }

        private IEnumerator KissingObject(Transform kissObj)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            EnableTippingToes(true);
            m_FBBIKHeadEffector.positionWeight = 0;
            m_FBBIKHeadEffector.VRMSetTarget(kissObj.transform);
            yield return StartCoroutine(WalkingToObject(kissObj, m_VRMBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
            float startKissingTime = Time.time;
            while ((Time.time - startKissingTime) / KissingDuraction < 0.8f && m_BodyStatus.IsGrounded(aszVRMBodyStatus.GroundedPart.Tipping))
            {
                m_FBBIKHeadEffector.positionWeight = KissingTowardTargetCurve.Evaluate((Time.time - startKissingTime) / KissingDuraction);
                yield return wait;
            }
        }
        #endregion
        #region Hold
        [ContextMenu("HoldObject")]
        private IEnumerator HoldingObject(aszUnityActionHold hold)
        {
            aszHoldable holdable = hold.TargetObject.GetComponentInChildren<aszHoldable>();
            if (holdable)
            {
                yield return StartCoroutine(HoldingObject(holdable));
            }
        }

        private IEnumerator HoldingObject(aszHoldable holdObj)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            yield return StartCoroutine(WalkingToObject(holdObj.transform, m_VRMBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
            bool leftHandReached = false, rightHandReached = false;
            StartCoroutine(CoroutineWithCallback(ReachingObject(holdObj.HoldTrans[0], HumanBodyBones.LeftHand), () => leftHandReached = true));
            StartCoroutine(CoroutineWithCallback(ReachingObject(holdObj.HoldTrans[1], HumanBodyBones.RightHand), () => rightHandReached = true));
            yield return new WaitUntil(() => leftHandReached && rightHandReached);
            m_BodyStatus.HoldingObj = holdObj;
        }
        #endregion
        #region Hug
        IEnumerator HugingObject(aszUnityActionHug hug)
        {
            aszHugable hugable = hug.TargetObject.GetComponentInChildren<aszHugable>();
            if (hugable)
            {
                yield return StartCoroutine(HugingObject(hugable));
            }
        }

        IEnumerator HugingObject(aszHugable hugObj)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            yield return StartCoroutine(WalkingToObject(hugObj.transform, hugObj.HugableDistance));
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
                    if (!m_BodyStatus.IsGrounded(aszVRMBodyStatus.GroundedPart.LeftToes))
                    {
                        Vector3 leftFootAngle = m_VRMBodyInfo.GetHumanoid.LeftFoot.eulerAngles;
                        if (leftFootAngle.x > 180)
                        {
                            leftFootAngle.x -= 360;
                        }
                        if (leftFootAngle.x < 35f)
                        {
                            m_LeftFootTarget.localRotation *= Quaternion.AngleAxis(Time.deltaTime * 200f, m_VRMBodyInfo.GetHumanoid.LeftFoot.localRotation * Vector3.right);
                        }
                    }

                    if (!m_BodyStatus.IsGrounded(aszVRMBodyStatus.GroundedPart.RightToes))
                    {
                        Vector3 rightFootAngle = m_VRMBodyInfo.GetHumanoid.RightFoot.localRotation.eulerAngles;
                        if (rightFootAngle.x > 180)
                        {
                            rightFootAngle.x -= 360;
                        }
                        if (rightFootAngle.x < 35f)
                        {
                            m_RightFootTarget.localRotation *= Quaternion.AngleAxis(Time.deltaTime * 200f, m_VRMBodyInfo.GetHumanoid.RightFoot.localRotation * Vector3.right);
                        }
                    }
                }
                yield return wait;
            }
        }
        #endregion
        #region Speak
        [ContextMenu("Speak")]
        public void Speak()
        {
            m_FriendSpeachController.Speak();
            StartCoroutine(LipSyncing());
        }

        private IEnumerator LipSyncing()
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            while (!m_FriendSpeachController.FinishedSpeaking)
            {
                if (m_FriendSpeachController.GetCurrentMouthShape(out MouthShape mouthShape) == Result.Success)
                {
                    Debug.Log("Exp1: " + mouthShape.Vowel1);
                    Debug.Log("Exp2: " + mouthShape.Vowel2);
                    m_VRMAnimationController.SetFacialExpressionBlend(mouthShape.ToFacialBlend());
                }
                else
                {
                    Debug.Log("Failed");
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
}
