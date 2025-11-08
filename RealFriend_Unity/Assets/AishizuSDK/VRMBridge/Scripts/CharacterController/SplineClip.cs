using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.Splines;
using UniHumanoid;
using System.Collections.Generic;
using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge
{

    public class SplineClip : MonoBehaviour
    {
        [SerializeField] private List<SplineClipSetting> m_SplineClipSettings;

        [Header("Init")]
        [SerializeField] private bool m_Inited = false;
        [SerializeField] private FullBodyBipedIK m_FullBodyBipedIK;
        [SerializeField] private FBBIKHeadEffector m_FBBIKHeadEffector;
        [SerializeField] private SplineContainer m_SplineContainer;
        [SerializeField] private Humanoid m_Humanoid;
        private int m_RunningControllers = 0; public bool IsFinished { get { return m_RunningControllers == 0 ? true : false; } }
        public void Init(FullBodyBipedIK bodyIK, FBBIKHeadEffector headEffector, Humanoid humanoid)
        {
            m_FBBIKHeadEffector = headEffector;
            m_FullBodyBipedIK = bodyIK;
            m_SplineContainer = GetComponent<SplineContainer>();
            m_Humanoid = humanoid;
            transform.parent = humanoid.GetBoneTransform(HumanBodyBones.Hips).parent;
            m_Inited = true;
        }

        [ContextMenu("Play")]
        public Result Play(bool reverse = false)
        {
            if (!m_Inited)
            {
                return Result.Failed;
            }

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            foreach (SplineClipSetting clipSetting in m_SplineClipSettings)
            {
                for (int i = 0; i < clipSetting.Controllers.Count; i++)
                {
                    if (!clipSetting.Controllers[i].Enable)
                    {
                        continue;
                    }
                    StartCoroutine(PlayingController(clipSetting, i, reverse));
                }
            }

            return Result.Success;
        }

        private List<IKEffector> influencedIKEffectors = new List<IKEffector>();
        private List<FBBIKHeadEffector> influencedHeadEffectors = new List<FBBIKHeadEffector>();

        public void Stop()
        {
            foreach(IKEffector ik in influencedIKEffectors)
            {
                ik.positionWeight = 0;
                ik.rotationWeight = 0;
            }

            foreach (FBBIKHeadEffector head in influencedHeadEffectors)
            {
                head.positionWeight = 0;
                head.rotationWeight = 0;
            }
        }

        [SerializeField] Pose parentPose;
        private IEnumerator PlayingController(SplineClipSetting clipSetting, int controllerIndex, bool reverse = false)
        {
            m_RunningControllers++;
            if (GetEffector(clipSetting.Controllers[controllerIndex].TargetEffector, out IKEffector targetEffector, out FBBIKHeadEffector targetHeadEffector) == Result.Success)
            {
                bool isHeadEffectorController = targetHeadEffector == null ? false : true;
                GameObject tmpEffectorTarget = new GameObject(clipSetting.Controllers[controllerIndex].TargetEffector.ToString() + "Target");
                Transform parent = m_Humanoid.GetBoneTransform(clipSetting.Parent);
                parentPose = new Pose(position: parent.position,
                                            rotation: parent.rotation);
                if (isHeadEffectorController)
                {
                    targetHeadEffector.transform.SetParent(tmpEffectorTarget.transform);
                    targetHeadEffector.transform.localPosition = Vector3.zero;
                    targetHeadEffector.transform.localRotation = Quaternion.identity;
                    influencedHeadEffectors.Add(targetHeadEffector);
                }
                else
                {
                    targetEffector.target = tmpEffectorTarget.transform;
                    influencedIKEffectors.Add(targetEffector);
                }

                float startPlayingTime = Time.time;
                float playingProgress = 0f;
                while (playingProgress < 1f)
                {
                    playingProgress = (Time.time - startPlayingTime) / clipSetting.Duration * clipSetting.Speed;
                    float actualProgress = reverse ? 1 - playingProgress : playingProgress;

                    // InverseLerp between 0~0.1f to have a fading in effect at the beginning
                    if (isHeadEffectorController)
                    {
                        targetHeadEffector.SetBendWeight(Mathf.InverseLerp(0, 0.1f, actualProgress) * 0.8f);
                        targetHeadEffector.positionWeight = Mathf.InverseLerp(0, 0.1f, actualProgress);
                        targetHeadEffector.rotationWeight = Mathf.InverseLerp(0, 0.1f, actualProgress);
                    }
                    else
                    {
                        targetEffector.positionWeight = Mathf.InverseLerp(0, 0.1f, actualProgress);
                        targetEffector.rotationWeight = Mathf.InverseLerp(0, 0.1f, actualProgress);
                    }

                    float clipProgress = clipSetting.Curve.Evaluate(actualProgress);
                    int usinSplineIndex = clipSetting.Controllers[controllerIndex].UsingSplineIndex;
                    #region Get local space pose
                    Pose localPose = new Pose();
                    localPose.position = m_SplineContainer.Splines[usinSplineIndex].EvaluatePosition(clipProgress);
                    if (isHeadEffectorController)
                    {
                        localPose.rotation = Quaternion.LookRotation(m_SplineContainer.Splines[usinSplineIndex].EvaluateTangent(clipProgress),
                                                                     m_SplineContainer.Splines[usinSplineIndex].EvaluateUpVector(clipProgress));
                    }
                    else
                    {
                        //Might want to change to use a helper ext function in the future
                        localPose.rotation = Quaternion.AngleAxis(angle: clipSetting.Controllers[controllerIndex].IntertXAxis ? 90f : -90f,
                                                                        axis: m_SplineContainer.Splines[usinSplineIndex].EvaluateUpVector(clipProgress))
                                           * Quaternion.LookRotation(m_SplineContainer.Splines[usinSplineIndex].EvaluateTangent(clipProgress),
                                                                        m_SplineContainer.Splines[usinSplineIndex].EvaluateUpVector(clipProgress));
                    }
                    #endregion
                    #region Get world space pose
                    Pose worldPose = new Pose();
                    worldPose.rotation = parentPose.rotation * localPose.rotation;
                    worldPose.position = parentPose.position + (parentPose.rotation * localPose.position);
                    #endregion
                    tmpEffectorTarget.transform.position = worldPose.position;
                    tmpEffectorTarget.transform.rotation = worldPose.rotation;
                    yield return aszUnityCoroutine.WaitForEndOfFrame;
                }
                if (isHeadEffectorController)
                {
                    targetHeadEffector.positionWeight = reverse ? 0 : 1;
                    targetHeadEffector.rotationWeight = reverse ? 0 : 1;
                    targetHeadEffector.SetBendWeight(reverse ? 0 : 0.8f);
                }
                else
                {
                    targetEffector.positionWeight = reverse ? 0 : 1;
                    targetEffector.rotationWeight = reverse ? 0 : 1;
                }
            }
            m_RunningControllers--;
        }

        private Result GetEffector(SplineClipSetting.Effector effector, out IKEffector returnEffector, out FBBIKHeadEffector returnHeadEffector)
        {
            returnEffector = null;
            returnHeadEffector = null;
            if (!m_Inited)
            {
                return Result.Failed;
            }
            switch (effector)
            {
                case SplineClipSetting.Effector.Head:
                    returnHeadEffector = m_FBBIKHeadEffector;
                    return Result.Success;
                case SplineClipSetting.Effector.LeftHand:
                    returnEffector = m_FullBodyBipedIK.solver.leftHandEffector;
                    break;
                case SplineClipSetting.Effector.RightHand:
                    returnEffector = m_FullBodyBipedIK.solver.rightHandEffector;
                    break;
                case SplineClipSetting.Effector.LeftFoot:
                    returnEffector = m_FullBodyBipedIK.solver.leftFootEffector;
                    break;
                case SplineClipSetting.Effector.RightFoot:
                    returnEffector = m_FullBodyBipedIK.solver.rightFootEffector;
                    break;
                default:
                    return Result.Failed;
            }
            return Result.Success;
        }
    }
}