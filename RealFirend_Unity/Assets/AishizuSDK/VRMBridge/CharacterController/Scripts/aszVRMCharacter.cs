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
        [SerializeField] aszVRMAnimationController m_VRMAnimationController;
        [SerializeField] aszVRMBodyInfo m_VRMBodyInfo;
        [SerializeField] FullBodyBipedIK m_FullBodyBipedIK;
        [SerializeField] FBBIKHeadEffector m_FBBIKHeadEffector;
        [SerializeField] aszSpeachController m_FriendSpeachController;
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
            m_FriendSpeachController = GetComponentInChildren<aszSpeachController>();
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
        public void WalkToTarget(aszVRMWalk walk)
        {
            StartCoroutine(WalkingToObject(walk));
        }

        private IEnumerator WalkingToObject(aszVRMWalk walk)
        {
            yield return StartCoroutine(WalkingToObject(walk.Walkable.transform, walk.StopDistance));
            walk.SetFinish(Result.Success);
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
        public void ReachObject(aszVRMReach reach)
        {
            StartCoroutine(ReachingObject(reach));
        }

        private IEnumerator ReachingObject(aszVRMReach reach)
        {
            yield return StartCoroutine(ReachingObject(reach.Reachable.transform, reach.Hand));
            reach.SetFinish(Result.Success);
        }

        private IEnumerator ReachingObject(Transform obj, HumanBodyBones usingJoint, bool undo = false)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            float startReachingTime = Time.time;
            IKEffector effector = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftHandEffector : m_FullBodyBipedIK.solver.rightHandEffector;
            IKMappingLimb mapping = usingJoint != HumanBodyBones.RightHand ? m_FullBodyBipedIK.solver.leftArmMapping : m_FullBodyBipedIK.solver.rightArmMapping;
            effector.VRMSetTarget(obj, m_FullBodyBipedIK);
            float progress = (Time.time - startReachingTime) / ReachingDuraction;
            while (progress < 1)
            {
                mapping.weight = ReachingTowardTargetCurve.Evaluate(undo ? 1 - progress : progress);
                effector.positionWeight = ReachingTowardTargetCurve.Evaluate(undo ? 1 - progress : progress);
                effector.rotationWeight = ReachingTowardTargetCurve.Evaluate(undo ? 1 - progress : progress);
                yield return wait;
                progress = (Time.time - startReachingTime) / ReachingDuraction;
            }
        }
        #endregion
        #region Touch

        public void TouchObject(aszVRMTouch touch)
        {
            StartCoroutine(TouchingObject(touch));
        }

        private IEnumerator TouchingObject(aszVRMTouch touch)
        {
            yield return StartCoroutine(TouchingObject(touch.Touchable.transform, touch.Hand));
            touch.SetFinish(Result.Success);
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

        public void SitOnObject(aszVRMSit sit)
        {
            StartCoroutine(SittingOnObject(sit));

        }

        private IEnumerator SittingOnObject(aszVRMSit sit)
        {
            yield return StartCoroutine(WalkingToObject(sit.Sitable.transform, 0.05f));
            yield return StartCoroutine(Facing(sit.Sitable.transform.position + sit.Sitable.transform.forward, 1f));
            m_VRMAnimationController.GetAnimator.SetTrigger("TriggerSit");
            yield return StartCoroutine(SitingElevatingToHeight(sit.Sitable.SitPose.position.y + m_VRMBodyInfo.GetBodyCode.HipRadious, 3f, m_VRMBodyInfo.GetHumanoid.GetBoneTransform(HumanBodyBones.Spine)));
            sit.SetFinish(Result.Success);
        }
        #endregion
        #region Kiss
        public void KissObject(aszVRMKiss kiss)
        {
            StartCoroutine(KissingObject(kiss));
        }

        private IEnumerator KissingObject(aszVRMKiss kiss)
        {
            YieldInstruction wait = new WaitForEndOfFrame();
            EnableTippingToes(true);
            m_FBBIKHeadEffector.positionWeight = 0;
            m_FBBIKHeadEffector.VRMSetTarget(kiss.Kissable.transform);
            yield return StartCoroutine(WalkingToObject(kiss.Kissable.transform, m_VRMBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
            float startKissingTime = Time.time;
            while ((Time.time - startKissingTime) / KissingDuraction < 0.8f && m_BodyStatus.IsGrounded(aszVRMBodyStatus.GroundedPart.Tipping))
            {
                m_FBBIKHeadEffector.positionWeight = KissingTowardTargetCurve.Evaluate((Time.time - startKissingTime) / KissingDuraction);
                yield return wait;
            }
            kiss.SetFinish(Result.Success);
        }
        #endregion
        #region Hold
        public void HoldObject(aszVRMHold hold)
        {
            StartCoroutine(HoldingObject(hold, 3f));
        }

        private IEnumerator HoldingObject(aszVRMHold hold, bool undo = false)
        {
            yield return StartCoroutine(WalkingToObject(hold.Holdable.transform, m_VRMBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
            bool leftHandReached = false, rightHandReached = false;
            StartCoroutine(CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[0], HumanBodyBones.LeftHand, undo), () => leftHandReached = true));
            StartCoroutine(CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[1], HumanBodyBones.RightHand, undo), () => rightHandReached = true));
            yield return new WaitUntil(() => leftHandReached && rightHandReached);
            m_BodyStatus.HoldingObj = undo ? null : hold.Holdable;
            hold.SetFinish(Result.Success);
        }

        private IEnumerator HoldingObject(aszVRMHold hold, float duraction = -1)
        {
            yield return StartCoroutine(WalkingToObject(hold.Holdable.transform, m_VRMBodyInfo.GetBodyCode.LeftArmLength * 0.8f));
            bool leftHandReached = false, rightHandReached = false;
            StartCoroutine(CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[0], HumanBodyBones.LeftHand), () => leftHandReached = true));
            StartCoroutine(CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[1], HumanBodyBones.RightHand), () => rightHandReached = true));
            yield return new WaitUntil(() => leftHandReached && rightHandReached);
            m_BodyStatus.HoldingObj = hold.Holdable;
            yield return aszUnityCoroutine.WaitForSeconds(duraction);
            bool leftHandUnReached = false, rightHandUnReached = false;
            StartCoroutine(CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[0], HumanBodyBones.LeftHand, true), () => leftHandUnReached = true));
            StartCoroutine(CoroutineWithCallback(ReachingObject(hold.Holdable.HoldTrans[1], HumanBodyBones.RightHand, true), () => rightHandUnReached = true));
            yield return new WaitUntil(() => leftHandUnReached && rightHandUnReached);
            m_BodyStatus.HoldingObj = null;
            hold.SetFinish(Result.Success);
        }
        #endregion
        #region Hug
        public void HugObject(aszVRMHug hug)
        {
            StartCoroutine(HugingObject(hug));
        }

        IEnumerator HugingObject(aszVRMHug hug)
        {
            yield return StartCoroutine(WalkingToObject(hug.Hugable.transform, hug.Hugable.HugableDistance));
            m_BodyStatus.HugingObj = hug.Hugable;
            yield return StartCoroutine(m_VRMAnimationController.PlayingHug());
            hug.SetFinish(Result.Success);
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
