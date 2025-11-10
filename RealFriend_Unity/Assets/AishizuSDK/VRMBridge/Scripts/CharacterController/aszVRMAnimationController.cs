using UnityEngine;
using System.Collections;
using Aishizu.UnityCore;
using Aishizu.Native;
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
            m_BodyInfo = GetComponentInChildren<aszVRMBodyInfo>();
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

        public void CleanAllExpression()
        {
            m_VRMInstance.Runtime.Expression.CleanAllExpression();
        }
        public void CleanEmotionExpression()
        {
            m_VRMInstance.Runtime.Expression.CleanEmotionExpression();
        }
        public void CleanVowelExpression()
        {
            m_VRMInstance.Runtime.Expression.CleanVowelExpression();
        }

        public void SetFacialExpression(FacialExpression expression, float weight)
        {
            m_VRMInstance.Runtime.Expression.SetWeight(expression.ToVRMExpressionKey(), weight);
        }

        [Range(0.1f, 5f)]
        public float FacialExpressionAmplifier = 1.0f;
        public void SetFacialExpressionBlend(FacialBlend expressionBlend)
        {
            m_VRMInstance.Runtime.Expression.SetWeight(expressionBlend.Expression1.ToVRMExpressionKey(), expressionBlend.Weight1 * FacialExpressionAmplifier);
            m_VRMInstance.Runtime.Expression.SetWeight(expressionBlend.Expression2.ToVRMExpressionKey(), expressionBlend.Weight2 * FacialExpressionAmplifier);
        }

        public IEnumerator PlayingHug(bool reverse = false)
        {
            SplineClip clip = Instantiate(m_HugClip).GetComponent<SplineClip>();
            clip.Init(m_FullBodyBipedIK, m_FBBIKHeadEffector, m_BodyInfo.GetHumanoid);
            if(clip.Play(reverse: reverse) == Result.Failed)
            {
                yield break;
            }
            while (!clip.IsFinished)
            {
                yield return aszUnityCoroutine.WaitForEndOfFrame;
            }
            if (reverse)
            {
                clip.Stop();
            }
        }
    }
}