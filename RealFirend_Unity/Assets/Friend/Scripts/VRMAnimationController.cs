using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;

public class VRMAnimationController : MonoBehaviour
{

    public float TravelingSpeed = 0;

    [Header("Configeration")]
    [SerializeField] private Animator m_Animator; public Animator GetAnimator => m_Animator;
    [SerializeField] FullBodyBipedIK m_FullBodyBipedIK;
    [SerializeField] FBBIKHeadEffector m_FBBIKHeadEffector;
    [SerializeField] BodyInfo m_BodyInfo;

    [Header("SplineClips")]
    [SerializeField] private GameObject m_HugClip;

    [Header("FacialExpression")]
    [SerializeField] private SkinnedMeshRenderer m_FaceMesh;

    void OnEnable()
    {
        m_FullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
        m_FBBIKHeadEffector = GetComponentInChildren<FBBIKHeadEffector>();
        m_BodyInfo = GetComponentInChildren<BodyInfo>();
    }

    void Update()
    {
        float lerpedTravelingSpeed = Mathf.InverseLerp(0, 0.2f, TravelingSpeed);
        m_Animator.SetFloat("TravelingSpeed", lerpedTravelingSpeed);
        m_Animator.SetFloat("MovementAnimSpeed", Mathf.Clamp(lerpedTravelingSpeed, 0.1f, 1f));
    }

    IEnumerator CaulatingMovingSpeed()
    {
        YieldInstruction wait = new WaitForFixedUpdate();
        Vector2 oldPos2D, newPos2D;
        oldPos2D = transform.position.ToLevelPosition();
        while (true)
        {
            newPos2D = transform.position.ToLevelPosition();
            TravelingSpeed = Vector2.Distance(newPos2D, oldPos2D) / Time.fixedDeltaTime;
            oldPos2D = newPos2D;
            yield return wait;
        }
    }

    public void SetFacialExpressionBlendShape(FacialExpression expression, bool controlMouth)
    {

    }

    [ContextMenu("PlayHugAnim")]
    public void PlayHug()
    {
        SplineClip clip = Instantiate(m_HugClip).GetComponent<SplineClip>();
        clip.Init(m_FullBodyBipedIK, m_FBBIKHeadEffector, m_BodyInfo.GetHumanoid);
        clip.Play();
    }
}
