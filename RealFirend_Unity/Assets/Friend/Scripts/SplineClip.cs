using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.Splines;
using UniHumanoid;
using System.Collections.Generic;

public class SplineClip : MonoBehaviour
{
    [SerializeField] private List<SplineClipSetting> m_SplineClipSettings;

    [Header("Init")]
    [SerializeField] private bool Inited = false;
    [SerializeField] private FullBodyBipedIK m_FullBodyBipedIK;
    [SerializeField] private FBBIKHeadEffector m_FBBIKHeadEffector;
    [SerializeField] private SplineContainer m_SplineContainer;
    [SerializeField] private Humanoid m_Humanoid;
    public void Init(FullBodyBipedIK bodyIK, FBBIKHeadEffector headEffector, Humanoid humanoid)
    {
        m_FBBIKHeadEffector = headEffector;
        m_FullBodyBipedIK = bodyIK;
        m_SplineContainer = GetComponent<SplineContainer>();
        m_Humanoid = humanoid;
        transform.parent = humanoid.GetBoneTransform(HumanBodyBones.Hips).parent;
        Inited = true;
    }

    [ContextMenu("Play")]
    public Result Play()
    {
        if (!Inited)
        {
            return Result.Failed;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        foreach(SplineClipSetting clipSetting in m_SplineClipSettings)
        {
            for (int i = 0; i < clipSetting.Controllers.Count; i++)
            {
                if (!clipSetting.Controllers[i].Enable)
                {
                    continue;
                }
                StartCoroutine(PlayingController(clipSetting, i));
            }
        }

        return Result.Success;
    }
    [SerializeField] Pose parentPose;
    private IEnumerator PlayingController(SplineClipSetting clipSetting, int controllerIndex)
    {
        YieldInstruction wait = new WaitForEndOfFrame();

        if (GetEffector(clipSetting.Controllers[controllerIndex].TargetEffector, out IKEffector targetEffector, out FBBIKHeadEffector targetHeadEffector) == Result.Success)
        {
            bool isHeadEffectorController = targetHeadEffector == null ? false : true;
            GameObject tmpEffectorTarget = new GameObject(clipSetting.Controllers[controllerIndex].TargetEffector.ToString() + "Target");
            Transform parent = m_Humanoid.GetBoneTransform(clipSetting.Parent);
            parentPose = new Pose( position: parent.position,
                                        rotation: parent.rotation); 
            if (isHeadEffectorController)
            {
                targetHeadEffector.transform.SetParent(tmpEffectorTarget.transform);
                targetHeadEffector.transform.localPosition = Vector3.zero;
                targetHeadEffector.transform.localRotation = Quaternion.identity;
                targetHeadEffector.positionWeight = 1f;
                targetHeadEffector.rotationWeight = 1f;
            }
            else
            {
                targetEffector.target = tmpEffectorTarget.transform;
                targetEffector.positionWeight = 1f;
                targetEffector.rotationWeight = 1f;
            }

            float startPlayingTime = Time.time;
            while ((Time.time - startPlayingTime) / clipSetting.Duration * clipSetting.Speed < 0.98f)
            {
                float clipProgress = clipSetting.Curve.Evaluate((Time.time - startPlayingTime) / clipSetting.Duration * clipSetting.Speed);
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
                    localPose.rotation =    Quaternion.AngleAxis(   angle: clipSetting.Controllers[controllerIndex].IntertXAxis ? 90f : -90f, 
                                                                    axis: m_SplineContainer.Splines[usinSplineIndex].EvaluateUpVector(clipProgress))
                                       *    Quaternion.LookRotation(m_SplineContainer.Splines[usinSplineIndex].EvaluateTangent(clipProgress),
                                                                    m_SplineContainer.Splines[usinSplineIndex].EvaluateUpVector(clipProgress));
                }
                #endregion
                #region Get wold space pose
                Pose worldPose = new Pose();
                worldPose.rotation = parentPose.rotation * localPose.rotation;
                Debug.Log("localRot: " + localPose.rotation.eulerAngles);
                Debug.Log("ParentRot: " + parentPose.rotation.eulerAngles);
                Debug.Log("WorldRot: " + worldPose.rotation.eulerAngles);
                worldPose.position = parentPose.position + (parentPose.rotation * localPose.position);
                #endregion
                tmpEffectorTarget.transform.position = worldPose.position;
                tmpEffectorTarget.transform.rotation = worldPose.rotation;
                Debug.Log("FinalWorldRot: " + tmpEffectorTarget.transform.rotation.eulerAngles);
                yield return wait;
            }
        }
    }

    private Result GetEffector(SplineClipSetting.Effector effector, out IKEffector returnEffector, out FBBIKHeadEffector returnHeadEffector)
    {
        returnEffector = null;
        returnHeadEffector = null;
        if (!Inited)
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
