using UnityEngine;
using System.Collections;
using Aishizu.UnityCore;
using RootMotion.FinalIK;
using UniVRM10;

namespace Aishizu.VRMBridge
{
    public class aszVRMAnimationController : MonoBehaviour
    {

        public float TravelingSpeed = 0;

        [Header("Configeration")]
        [SerializeField] private Animator m_Animator; public Animator GetAnimator => m_Animator;
        [SerializeField] private FullBodyBipedIK m_FullBodyBipedIK;
        [SerializeField] private FBBIKHeadEffector m_FBBIKHeadEffector;
        [SerializeField] private aszVRMBodyInfo m_BodyInfo;
        [SerializeField] private Vrm10Instance m_VRMInstance;

        [Header("SplineClips")]
        [SerializeField] private GameObject m_HugClip;

        private void OnEnable()
        {
            m_Animator = GetComponentInChildren<Animator>();
            m_FullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
            m_FBBIKHeadEffector = GetComponentInChildren<FBBIKHeadEffector>();
            m_VRMInstance = GetComponentInChildren<Vrm10Instance>();
        }

        private void Update()
        {
            float lerpedTravelingSpeed = Mathf.InverseLerp(0, 0.2f, TravelingSpeed);
            m_Animator.SetFloat("TravelingSpeed", lerpedTravelingSpeed);
            m_Animator.SetFloat("MovementAnimSpeed", Mathf.Clamp(lerpedTravelingSpeed, 0.1f, 1f));
        }

        private IEnumerator CaulatingMovingSpeed()
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

        [SerializeField] FacialExpression de_Expression;
        [SerializeField][Range(0, 1.0f)] float de_ExpressionWeight;
        [ContextMenu("DoExpression")]
        public void de_DoExpression()
        {
            SetFacialExpression(de_Expression, de_ExpressionWeight);
        }

        public void CleanExpression()
        {
            m_VRMInstance.Runtime.Expression.CleanExpression();
        }

        public void SetFacialExpression(FacialExpression expression, float weight)
        {
            m_VRMInstance.Runtime.Expression.SetWeight(expression.ToVRMExpressionKey(), weight);
        }

        public void SetFacialExpression(FacialExpression expression)
        {
            m_VRMInstance.Runtime.Expression.CleanExpression();
            if (expression.ExtractExpression(out FacialExpression resultExpression) == Result.Success)
            {
                m_VRMInstance.Runtime.Expression.SetWeight(resultExpression.ToVRMExpressionKey(), 1f);
            }

            if (expression.ExtractVowel(out FacialExpression resultVowel) == Result.Success)
            {
                m_VRMInstance.Runtime.Expression.SetWeight(resultVowel.ToVRMExpressionKey(), 1f);
            }
        }

        public void SetFacialExpressionBlend(FacialBlend expressionBlend)
        {
            m_VRMInstance.Runtime.Expression.CleanExpression();
            m_VRMInstance.Runtime.Expression.SetWeight(expressionBlend.Expression1.ToVRMExpressionKey(), expressionBlend.Weight1);
            m_VRMInstance.Runtime.Expression.SetWeight(expressionBlend.Expression2.ToVRMExpressionKey(), expressionBlend.Weight2);
        }

        [ContextMenu("PlayHugAnim")]
        public void PlayHug()
        {
            SplineClip clip = Instantiate(m_HugClip).GetComponent<SplineClip>();
            clip.Init(m_FullBodyBipedIK, m_FBBIKHeadEffector, m_BodyInfo.GetHumanoid);
            clip.Play();
        }
    }
}