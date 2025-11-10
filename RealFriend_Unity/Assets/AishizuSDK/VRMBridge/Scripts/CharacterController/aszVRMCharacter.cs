using UnityEngine;
using System.Collections;
using System;
using RootMotion.FinalIK;
using Aishizu.UnityCore;
using Aishizu.UnityCore.Speach;
using Aishizu.VRMBridge.Actions;
using Aishizu.Native;

namespace Aishizu.VRMBridge
{
    public class aszVRMCharacter : aszActor
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
        [SerializeField] aszVRMAnimationController m_VRMAnimationController; public aszVRMAnimationController VRMAnimationController => m_VRMAnimationController;
        [SerializeField] aszVRMBodyInfo m_VRMBodyInfo;
        [SerializeField] FullBodyBipedIK m_FullBodyBipedIK;
        [SerializeField] FBBIKHeadEffector m_FBBIKHeadEffector;
        [SerializeField] aszSpeachController m_SpeachController; public aszSpeachController SpeachController => m_SpeachController;
        [SerializeField] aszVRMBodyStatus m_BodyStatus;
        [Header("AutoConfigeration")]
        [SerializeField] Transform m_LeftHandTarget;
        [SerializeField] Transform m_RightHandTarget;
        [SerializeField] Transform m_LeftFootTarget;
        [SerializeField] Transform m_RightFootTarget;

        void OnEnable()
        {
            m_VRMBodyInfo = GetComponent<aszVRMBodyInfo>();
            m_FullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
            m_VRMAnimationController = GetComponentInChildren<aszVRMAnimationController>();
            m_FBBIKHeadEffector = GetComponentInChildren<FBBIKHeadEffector>();
            m_SpeachController = GetComponentInChildren<aszSpeachController>();
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

        #region Walk
        /*
        public void WalkToTarget(aszVRMWalk walk)
        {
            StartCoroutine(WalkingToObject(walk));
        }

        private IEnumerator WalkingToObject(aszVRMWalk walk)
        {
            yield return StartCoroutine(WalkingToObject(walk.Walkable.transform, walk.StopDistance));
            walk.SetState(aszActionState.);
        }
        */
        private IEnumerator WalkingToObject(Transform obj, float stopDistance)
        {
            yield return StartCoroutine(WalkingToPosition(obj.position, stopDistance));
        }

        private IEnumerator WalkingInFrontOfObject(Transform obj, float frontDistance, float stopDistance, Transform lookAtObj = null)
        {
            yield return StartCoroutine(WalkingToPosition(obj.position + obj.forward * frontDistance, stopDistance));
            yield return StartCoroutine(Face(obj.position, 0.2f));
        }

        private IEnumerator WalkingToPosition(Vector3 pos, float stopDistance)
        {
            float distance = Vector3.Distance(pos.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));

            while (distance > stopDistance)
            {
                WalkToPos(pos, stopDistance, ref distance);
                yield return aszUnityCoroutine.WaitForEndOfFrame;
            }
            FinalMovingSpeed = 0;
        }
        private void WalkToPos(Vector3 pos, float distanceThreshold, ref float distance)
        {
            transform.LookAt(pos.ToLevelPosition(transform), Vector3.up);
            FinalMovingSpeed = NormalMovingSpeed * MovingSpeedTowardTargetCurve.Evaluate(distance - (distanceThreshold * 0.75f));
            transform.position += (pos.ToLevelPosition(transform) - transform.position.ToLevelPosition(transform)).normalized * Time.deltaTime * FinalMovingSpeed;
            distance = Vector3.Distance(pos.ToLevelPosition(transform), transform.position.ToLevelPosition(transform));
        }
        #endregion
        #region Facing
        private IEnumerator Face(Vector3 pos, float withInDuraction)
        {
            float angleDiff = Vector3.SignedAngle(transform.forward, (pos - transform.position).normalized, Vector3.up);
            float startFacingTime = Time.time;
            Quaternion originalRotation = transform.rotation;
            while (Time.time - startFacingTime < withInDuraction)
            {
                float currentAngle = angleDiff * FacingTargetCurve.Evaluate((Time.time - startFacingTime) / withInDuraction);
                transform.rotation = originalRotation * Quaternion.AngleAxis(currentAngle, Vector3.up); ;
                yield return aszUnityCoroutine.WaitForEndOfFrame;
            }
            transform.LookAt(pos.ToLevelPosition(transform), Vector3.up);
        }

        #endregion
        #region LookAt
        private IEnumerator LookingAtObject(Transform target)
        {
            while (true)
            {
                LookAtObject(target);
                yield return aszUnityCoroutine.WaitForEndOfFrame;
            }
        }

        private void LookAtObject(Transform target)
        {
            m_FBBIKHeadEffector.rotationWeight = target ? 1f : 0f;
            m_FBBIKHeadEffector.SetBendWeight(target ? 0.8f : 0f);
            m_FBBIKHeadEffector.transform.rotation = Quaternion.LookRotation((target.position - m_VRMBodyInfo.GetSupportJoints.HeadCenter.position).normalized, m_VRMBodyInfo.GetSupportJoints.HeadCenter.up);
        }
        #endregion
        #region Reach
        /*
        public void ReachObject(aszVRMReach reach)
        {
            StartCoroutine(ReachingObject(reach));
        }

        private IEnumerator ReachingObject(aszVRMReach reach)
        {
            yield return StartCoroutine(ReachingObject(reach.Reachable.transform, reach.Hand));
            reach.SetState(aszActionState.Running);
        }
        */
        private IEnumerator ReachingObject(Transform obj, HumanBodyBones usingJoint, bool undo = false)
        {
            float startReachingTime = Time.time;
            IKEffector effector = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftHandEffector : m_FullBodyBipedIK.solver.rightHandEffector;
            IKMappingLimb mapping = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftArmMapping : m_FullBodyBipedIK.solver.rightArmMapping;
            effector.VRMSetTarget(obj, m_FullBodyBipedIK);
            float progress = 0;
            while (progress < 1)
            {
                mapping.weight = ReachingTowardTargetCurve.Evaluate(undo ? 1 - progress : progress);
                effector.positionWeight = ReachingTowardTargetCurve.Evaluate(undo ? 1 - progress : progress);
                effector.rotationWeight = ReachingTowardTargetCurve.Evaluate(undo ? 1 - progress : progress);
                yield return aszUnityCoroutine.WaitForEndOfFrame;
                progress = (Time.time - startReachingTime) / ReachingDuraction;
            }

            effector.positionWeight = undo ? 0 : 1;
            effector.rotationWeight = undo ? 0 : 1;
        }
        #endregion
        #region Touch

        public void TouchObject(aszVRMTouch touch, bool undo)
        {
            StartCoroutine(TouchingObject(touch, undo: undo));
        }

        private IEnumerator TouchingObject(aszVRMTouch touch, bool undo = false)
        {
            yield return StartCoroutine(TouchingObject(touch.Touchable.transform, touch.Hand, undo));
            touch.ProgressStage();
        }

        private IEnumerator TouchingObject(Transform obj, HumanBodyBones usingJoint, bool undo = false)
        {
            yield return StartCoroutine(WalkingToObject(obj, (usingJoint != HumanBodyBones.RightHand ? m_VRMBodyInfo.GetBodyCode.LeftArmLength : m_VRMBodyInfo.GetBodyCode.RightArmLength) * 0.8f));
            yield return StartCoroutine(ReachingObject(obj, usingJoint, undo));
        }
        #endregion
        #region Sit
        public void SitOnObject(aszVRMSit sit, bool undo)
        {
            StartCoroutine(SittingOnObject(sit, undo: undo));
        }
        private IEnumerator SittingOnObject(aszVRMSit sit, bool setFinish = true, bool undo = false)
        {
            if (undo)
            {
                yield return StartCoroutine(WalkingToPosition(sit.Sitable.transform.position + sit.Sitable.transform.forward * sit.Sitable.Edge, 0.05f));
            }
            else
            {
                yield return StartCoroutine(WalkingToObject(sit.Sitable.transform, 0.05f));
                yield return StartCoroutine(Face(sit.Sitable.transform.position + sit.Sitable.transform.forward, 1f));
            }
            m_VRMAnimationController.GetAnimator.SetTrigger("TriggerSit");
            yield return StartCoroutine(SitingElevatingToHeight(sit.Sitable.SitPose.position.y + m_VRMBodyInfo.GetBodyCode.HipRadious, 3f, m_VRMBodyInfo.GetHumanoid.GetBoneTransform(HumanBodyBones.Spine)));
            sit.ProgressStage(); 
        }

        private Vector3 originalSitPos;
        private IEnumerator SitingElevatingToHeight(float targetHeight, float withInDuraction, Transform hip, bool undo = false)
        {
            float startElevatingTime = Time.time;
            if (!undo)
            {
                originalSitPos = transform.position;
            }
            float progress = 0;
            while (progress < 1)
            {
                transform.position = originalSitPos + Vector3.up * (targetHeight - hip.position.y) * SittingElevateCurve.Evaluate(undo ? (1 - progress) : progress);
                yield return aszUnityCoroutine.WaitForEndOfFrame;
                progress = (Time.time - startElevatingTime) / withInDuraction;
            }

            transform.position = undo ? originalSitPos : originalSitPos + Vector3.up * (targetHeight - hip.position.y);
        }
        #endregion
        #region Kiss
        public void KissObject(aszVRMKiss kiss, bool undo)
        {
            StartCoroutine(KissingObject(kiss, undo: undo));
        }
        private IEnumerator KissingObject(aszVRMKiss kiss, bool setFinish = true, bool undo = false)
        {
            if (!undo)
            {
                m_FBBIKHeadEffector.positionWeight = 0;
                m_FBBIKHeadEffector.rotationWeight = 0;
                m_FBBIKHeadEffector.VRMSetTarget(kiss.Kissable.transform);
                yield return StartCoroutine(WalkingToObject(kiss.Kissable.transform, m_VRMBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
            }
            float startKissingTime = Time.time;
            float progress = 0;
            while (progress < 1)
            {
                m_FBBIKHeadEffector.positionWeight = KissingTowardTargetCurve.Evaluate(undo ? (1 - progress) : progress);
                m_FBBIKHeadEffector.rotationWeight = KissingTowardTargetCurve.Evaluate(undo ? (1 - progress) : progress);
                m_FBBIKHeadEffector.SetBendWeight(Mathf.InverseLerp(0, 0.1f, undo ? (1 - progress) : progress) * 0.8f);
                yield return aszUnityCoroutine.WaitForEndOfFrame;
                progress = (Time.time - startKissingTime) / KissingDuraction;
            }
            m_FBBIKHeadEffector.positionWeight = undo ? 0 : 1;
            m_FBBIKHeadEffector.rotationWeight = undo ? 0 : 1;
            m_FBBIKHeadEffector.SetBendWeight(undo ? 0 : 0.8f);
            kiss.ProgressStage();
        }
        #endregion
        #region Hold
        public void HoldObject(aszVRMHold hold, bool undo)
        {
            StartCoroutine(HoldingObject(hold, undo: undo));
        }

        private IEnumerator HoldingObject(aszVRMHold hold, bool undo = false)
        {
            if (!undo)
            {
                yield return StartCoroutine(WalkingInFrontOfObject(hold.Holdable.transform, 1f, 0.1f));
                yield return StartCoroutine(WalkingToObject(hold.Holdable.transform, m_VRMBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
            }
            bool leftHandReached = false, rightHandReached = false;
            StartCoroutine(this.CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[0], HumanBodyBones.LeftHand, undo), () => leftHandReached = true));
            StartCoroutine(this.CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[1], HumanBodyBones.RightHand, undo), () => rightHandReached = true));
            yield return new WaitUntil(() => leftHandReached && rightHandReached);
            hold.ProgressStage();
        }
        #endregion
        #region Hug
        public void HugObject(aszVRMHug hug, bool undo)
        {
            StartCoroutine(HugingObject(hug, undo: undo));
        }

        private IEnumerator HugingObject(aszVRMHug hug, bool undo)
        {
            if (!undo)
            {
                if (hug.Hugable.GetVRMBodyInfo(out aszVRMBodyInfo bodyInfo) == Result.Success)
                {
                    yield return StartCoroutine(WalkingInFrontOfObject(hug.Hugable.transform, 1f, 0.1f, lookAtObj: bodyInfo.GetSupportJoints.HeadCenter));
                }
                else
                {
                    yield return StartCoroutine(WalkingInFrontOfObject(hug.Hugable.transform, 1f, 0.1f));
                }
                yield return StartCoroutine(WalkingToObject(hug.Hugable.transform, hug.Hugable.HugableDistance));
            }
            yield return StartCoroutine(m_VRMAnimationController.PlayingHug(reverse: undo));
            hug.ProgressStage();
        }
        #endregion
        #region TipToes
        private bool EnableTipping = false;
        private Quaternion[] footOriginalRotations = new Quaternion[2];
        private void EnableTippingToes(bool enable)
        {
            if (enable)
            {
                m_FullBodyBipedIK.solver.leftFootEffector.rotationWeight = 1f;
                m_FullBodyBipedIK.solver.rightFootEffector.rotationWeight = 1f;
                footOriginalRotations[0] = m_RightFootTarget.localRotation;
                footOriginalRotations[1] = m_LeftFootTarget.localRotation;
                m_LeftFootTarget.localRotation = Quaternion.identity;
                m_RightFootTarget.localRotation = Quaternion.identity;
            }
            else
            {
                m_FullBodyBipedIK.solver.leftFootEffector.rotationWeight = 0f;
                m_FullBodyBipedIK.solver.rightFootEffector.rotationWeight = 0f;
                m_LeftFootTarget.localRotation = footOriginalRotations[1];
                m_RightFootTarget.localRotation = footOriginalRotations[0];
            }
            EnableTipping = enable;
        }
        IEnumerator TippingToes()
        {
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
                yield return aszUnityCoroutine.WaitForEndOfFrame;
            }
        }
        #endregion
        #region Speak
        [ContextMenu("de_Speak")]
        public void Speak()
        {
            Speak("Hey, I missed you.");
        }
        public void Speak(string text)
        {
            m_SpeachController.Speak(text);
            StartCoroutine(LipSyncing());
        }

        private IEnumerator LipSyncing()
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            while (!m_SpeachController.FinishedSpeaking)
            {
                if (m_SpeachController.GetCurrentMouthShape(out MouthShape mouthShape) == Result.Success)
                {
                    m_VRMAnimationController.CleanVowelExpression();
                    m_VRMAnimationController.SetFacialExpressionBlend(mouthShape.ToFacialBlend());
                }
                yield return wait;
            }
            m_VRMAnimationController.SetFacialExpressionBlend(FacialBlend.Default);
        }
        #endregion
    }
}
