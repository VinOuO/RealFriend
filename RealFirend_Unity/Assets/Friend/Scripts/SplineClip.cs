using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.Splines;
using UniHumanoid;

public class SplineClip : MonoBehaviour
{
    [SerializeField] private SplineClipSetting m_SplineClipSetting;

    [Header("Init")]
    [SerializeField] private bool Inited = false;
    [SerializeField] private FullBodyBipedIK m_FullBodyBipedIK;
    [SerializeField] private FBBIKHeadEffector m_FBBIKHeadEffector;
    [SerializeField] private SplineContainer m_SplineContainer;
    public void Init(FullBodyBipedIK bodyIK, FBBIKHeadEffector headEffector, Humanoid humanoid)
    {
        m_FBBIKHeadEffector = headEffector;
        m_FullBodyBipedIK = bodyIK;
        m_SplineContainer = GetComponent<SplineContainer>();
        transform.parent = humanoid.GetBoneTransform(m_SplineClipSetting.Parent);
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

        for(int i = 0; i < m_SplineClipSetting.Controllers.Count; i++)
        {
            StartCoroutine(PlayingController(i));
        }
        return Result.Success;
    }

    private IEnumerator PlayingController(int controllerIndex)
    {
        YieldInstruction wait = new WaitForEndOfFrame();

        if (GetEffector(m_SplineClipSetting.Controllers[controllerIndex].TargetEffector, out IKEffector targetEffector) == Result.Success)
        {
            GameObject tmpEffectorTarget = new GameObject(m_SplineClipSetting.Controllers[controllerIndex].TargetEffector.ToString() + "Target");
            tmpEffectorTarget.transform.SetParent(transform);
            targetEffector.target = tmpEffectorTarget.transform;
            targetEffector.positionWeight = 1;
            targetEffector.rotationWeight = 1;
            float startPlayingTime = Time.time;
            while ((Time.time - startPlayingTime) / m_SplineClipSetting.Duration < 0.95f)
            {
                float clipProgress = m_SplineClipSetting.Curve.Evaluate((Time.time - startPlayingTime) / m_SplineClipSetting.Duration);
                tmpEffectorTarget.transform.localPosition = m_SplineContainer.Splines[controllerIndex].EvaluatePosition(clipProgress);
                Quaternion tmpRotation = Quaternion.AngleAxis(m_SplineClipSetting.Controllers[controllerIndex].IntertXAxis ? 90f : -90f, m_SplineContainer.Splines[controllerIndex].EvaluateUpVector(clipProgress)) * Quaternion.LookRotation(m_SplineContainer.Splines[controllerIndex].EvaluateTangent(clipProgress),
                                                                                    m_SplineContainer.Splines[controllerIndex].EvaluateUpVector(clipProgress));
                tmpEffectorTarget.transform.localRotation = tmpRotation;
                Debug.Log("Progress: " + clipProgress);
                yield return wait;
            }
        }
    }

    private Result GetEffector(SplineClipSetting.Effector effector, out IKEffector returnEffector)
    {
        if (!Inited)
        {
            returnEffector = null;
            return Result.Failed;
        }
        switch (effector)
        {
            case SplineClipSetting.Effector.Head:
                returnEffector = null;
                return Result.Failed;
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
                returnEffector = null;
                return Result.Failed;
        }
        return Result.Success;
    }
}
