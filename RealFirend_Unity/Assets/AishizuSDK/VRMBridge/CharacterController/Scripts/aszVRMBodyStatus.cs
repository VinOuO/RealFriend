using UnityEngine;

namespace Aishizu.VRMBridge
{
    public class aszVRMBodyStatus : MonoBehaviour
    {
        [Header("AutoConfig")]
        [SerializeField] private aszVRMBodyInfo m_BodyInfo;
        [SerializeField] private aszVRMFootController m_LeftBodyFootStatus, m_RightBodyFootStatus;
        [SerializeField] private aszVRMBodyToesController m_LeftBodyToesStatus, m_RightBodyToesStatus;

        [Header("Status")]
        [SerializeField] private aszHoldable m_HoldingObj; public aszHoldable HoldingObj { get { return m_HoldingObj; } set { m_HoldingObj = value; } }
        [SerializeField] private aszHugable m_HugingObj; public aszHugable HugingObj { get { return m_HugingObj; } set { m_HugingObj = value; } }
        public bool IsHolding { get { return m_HoldingObj == null ? false : true; } }
        public bool IsHuging { get { return m_HugingObj == null ? false : true; } }
        private void OnEnable()
        {
            m_BodyInfo = GetComponent<aszVRMBodyInfo>();
            Init();
        }

        private void Init()
        {
            SetCollider(m_BodyInfo.GetHumanoid.RightFoot, m_BodyInfo.GetHumanoid.RightToes.localPosition.y * 1.8f);
            SetCollider(m_BodyInfo.GetHumanoid.LeftFoot, m_BodyInfo.GetHumanoid.LeftToes.localPosition.y * 1.8f);
            m_LeftBodyFootStatus = m_BodyInfo.GetHumanoid.LeftFoot.gameObject.AddComponent<aszVRMFootController>();
            m_RightBodyFootStatus = m_BodyInfo.GetHumanoid.RightFoot.gameObject.AddComponent<aszVRMFootController>();
            m_LeftBodyToesStatus = m_BodyInfo.GetHumanoid.LeftToes.gameObject.AddComponent<aszVRMBodyToesController>();
            m_RightBodyToesStatus = m_BodyInfo.GetHumanoid.RightToes.gameObject.AddComponent<aszVRMBodyToesController>();

            //Might want to change to use an accurate alogirthm (e.g. raycasting to the ground)
            SetCollider(m_BodyInfo.GetHumanoid.LeftToes, 0.02f);
            SetCollider(m_BodyInfo.GetHumanoid.RightToes, 0.02f);


        }

        private void SetCollider(Transform target, float radius)
        {
            SphereCollider tmpCollider = target.gameObject.AddComponent<SphereCollider>();
            tmpCollider.radius = Mathf.Abs(radius);
            tmpCollider.isTrigger = true;
            target.gameObject.AddComponent<Rigidbody>();
        }

        public bool IsGrounded(GroundedPart part)
        {
            switch (part)
            {
                case GroundedPart.LeftFoot:
                    return m_LeftBodyFootStatus.IsGounded;
                case GroundedPart.RightFoot:
                    return m_RightBodyFootStatus.IsGounded;
                case GroundedPart.LeftToes:
                    return m_LeftBodyToesStatus.IsGounded;
                case GroundedPart.RightToes:
                    return m_RightBodyToesStatus.IsGounded;
                case GroundedPart.BothFoot:
                    return m_LeftBodyFootStatus.IsGounded || m_RightBodyFootStatus.IsGounded;
                case GroundedPart.BothToes:
                    return m_LeftBodyToesStatus.IsGounded || m_RightBodyToesStatus.IsGounded;
                case GroundedPart.Tipping:
                    float leftAngle = m_LeftBodyFootStatus.transform.localRotation.eulerAngles.x;
                    float rightAngle = m_RightBodyFootStatus.transform.localRotation.eulerAngles.x;
                    if (leftAngle > 180)
                    {
                        leftAngle -= 360;
                    }
                    if (rightAngle > 180)
                    {
                        rightAngle -= 360;
                    }
                    return IsGrounded(GroundedPart.BothFoot) || leftAngle < 25f || rightAngle < 25f;
                case GroundedPart.All:
                    return IsGrounded(GroundedPart.BothFoot) || IsGrounded(GroundedPart.BothToes);
            }
            return false;
        }

        public enum GroundedPart
        {
            LeftFoot, RightFoot,
            LeftToes, RightToes,
            BothFoot, BothToes,
            Tipping, All,
        }
    }
}

