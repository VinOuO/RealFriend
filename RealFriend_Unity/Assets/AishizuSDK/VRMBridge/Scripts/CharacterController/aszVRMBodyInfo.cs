using UnityEngine;
using UniHumanoid;
using UniVRM10;
using Aishizu.UnityCore;
using Aishizu.Native;

namespace Aishizu.VRMBridge
{
    public class aszVRMBodyInfo : MonoBehaviour
    {
        [Header("AutoConfig")]
        [SerializeField] private Humanoid m_Humanoid; public Humanoid GetHumanoid => m_Humanoid;

        [Header("Infos")]
        [SerializeField] private BodyCode m_BodyCode = BodyCode.Default; public BodyCode GetBodyCode => m_BodyCode;

        [SerializeField] private SupportJoints m_SupportJoints; public SupportJoints GetSupportJoints => m_SupportJoints;

        [Header("Debug")]
        public Vector3 LeftCheekDetectAngle;
        public Vector3 RightCheekDetectAngle;
        public Vector3 MouthDetectAngle;

        private void OnEnable()
        {
            m_Humanoid = GetComponent<Humanoid>();
            UpdateLimbInfo();
            SetSupportJoints();
        }

        void UpdateLimbInfo()
        {
            m_BodyCode = BodyCode.Zero;
            #region Limb
            m_BodyCode.LeftArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm);
            m_BodyCode.LeftArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);

            m_BodyCode.RightArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm);
            m_BodyCode.RightArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);

            m_BodyCode.LeftLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg);
            m_BodyCode.LeftLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);

            m_BodyCode.RightLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg);
            m_BodyCode.RightLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);
            #endregion
            if (m_Humanoid.GetComponentFromJoint<VRM10SpringBoneCollider>(HumanBodyBones.LeftUpperLeg, out VRM10SpringBoneCollider leftSpringBone) == Result.Success &&
               m_Humanoid.GetComponentFromJoint<VRM10SpringBoneCollider>(HumanBodyBones.LeftUpperLeg, out VRM10SpringBoneCollider rightSpringBone) == Result.Success)
            {
                m_BodyCode.HipRadious = Vector3.Distance(m_Humanoid.GetJointPosition(HumanBodyBones.Spine),
                                                        (m_Humanoid.GetJointPosition(HumanBodyBones.LeftUpperLeg) + m_Humanoid.GetJointPosition(HumanBodyBones.RightUpperLeg)) / 2.0f)
                                        + (leftSpringBone.Radius + rightSpringBone.Radius) / 2.0f;
            }
            else
            {
                m_BodyCode.HipRadious = Vector3.Distance(m_Humanoid.GetJointPosition(HumanBodyBones.Spine),
                                                       (m_Humanoid.GetJointPosition(HumanBodyBones.LeftUpperLeg) + m_Humanoid.GetJointPosition(HumanBodyBones.RightUpperLeg)) / 2.0f);
                m_BodyCode.HipRadious *= 1.2f;
            }
        }

        [ContextMenu("SetSupportJoints")]
        public void de_SetSupportJoints()
        {
            Debug.Log(SetSupportJoints());
        }
        Result SetSupportJoints()
        {
            #region GetReferences
            if (!m_Humanoid)
            {
                Debug.LogError("No Humanoid");
                return Result.Failed;
            }
            GameObject face = gameObject.FindInChild("Face");
            if (!face)
            {
                Debug.LogError("No Face");
                return Result.Failed;
            }
            MeshCollider faceCollider = face.GetComponent<MeshCollider>();
            if (!faceCollider)
            {
                faceCollider = face.AddComponent<MeshCollider>();
            }
            SkinnedMeshRenderer faceRenderer = face.GetComponent<SkinnedMeshRenderer>();

            if (!faceCollider || !faceRenderer)
            {
                Debug.LogError("No Face Components");
                return Result.Failed;
            }
            #endregion
            #region SetHeadCenterPosition
            m_SupportJoints.Clear();
            m_SupportJoints = new SupportJoints(m_Humanoid.Head);
            m_SupportJoints.HeadCenter.position = m_Humanoid.Head.GetComponent<VRM10SpringBoneCollider>().Offset + m_Humanoid.Head.position;
            m_SupportJoints.HeadCenter.rotation = m_Humanoid.Head.rotation;
            #endregion
            #region SetCheeksPosition
            faceCollider.convex = false;
            faceCollider.sharedMesh = faceRenderer.sharedMesh;
            faceCollider.gameObject.layer = LayerMask.NameToLayer("Face");

            Vector3 leftCheekCastFrom = m_SupportJoints.HeadCenter.position + GetDetectDirection(LeftCheekDetectAngle, m_SupportJoints.HeadCenter);
            Ray tmpRay = new Ray(leftCheekCastFrom, m_SupportJoints.HeadCenter.position - leftCheekCastFrom);

            RaycastHit[] hits = Physics.RaycastAll(ray: tmpRay,
                                                   maxDistance: 10f,
                                                   layerMask: ~LayerMask.NameToLayer("Face"));
            if (hits.Length > 0)
            {
                bool set = false;
                foreach (RaycastHit hit in hits)
                {
                    if (gameObject.IsParentOf(hit.collider.gameObject))
                    {
                        m_SupportJoints.LeftCheek.position = hit.point;
                        Vector3 up = hit.normal;
                        Vector3 shoulderToHand = m_Humanoid.LeftHand.position - m_Humanoid.LeftLowerArm.position;
                        Vector3 forward = Vector3.Cross(up, Vector3.Cross(shoulderToHand, up));
                        if (forward == Vector3.zero)
                        {
                            forward = Vector3.Cross(up, Vector3.forward);
                        }
                        m_SupportJoints.LeftCheek.rotation = Quaternion.LookRotation(forward.normalized, up);
                        set = true;
                        break;
                    }
                }
                if (!set)
                {
                    Debug.LogError("Didn't hit face");
                    return Result.Failed;
                }
            }
            else
            {
                Debug.LogError("Didn't hit anything");
                return Result.Failed;
            }

            Vector3 rightCheekCastFrom = m_SupportJoints.HeadCenter.position + GetDetectDirection(RightCheekDetectAngle, m_SupportJoints.HeadCenter);
            tmpRay = new Ray(rightCheekCastFrom, m_SupportJoints.HeadCenter.position - rightCheekCastFrom);
            hits = Physics.RaycastAll(ray: tmpRay,
                                      maxDistance: 10f,
                                      layerMask: ~LayerMask.NameToLayer("Face"));
            if (hits.Length > 0)
            {
                bool set = false;
                foreach (RaycastHit hit in hits)
                {
                    if (gameObject.IsParentOf(hit.collider.gameObject))
                    {
                        m_SupportJoints.RightCheek.position = hit.point;
                        Vector3 up = hit.normal;
                        Vector3 shoulderToHand = m_Humanoid.RightHand.position - m_Humanoid.RightLowerArm.position;
                        Vector3 forward = Vector3.Cross(up, Vector3.Cross(shoulderToHand, up));
                        if (forward == Vector3.zero)
                        {
                            forward = Vector3.Cross(up, Vector3.forward);
                        }
                        m_SupportJoints.RightCheek.rotation = Quaternion.LookRotation(forward.normalized, up);
                        set = true;
                        break;
                    }
                }
                if (!set)
                {
                    Debug.LogError("Didn't hit face");
                    return Result.Failed;
                }
            }
            else
            {
                Debug.LogError("Didn't hit anything");
                return Result.Failed;
            }
            #endregion
            #region SetMouthPosition
            faceCollider.convex = true;
            Vector3 mouthCastFrom = m_SupportJoints.HeadCenter.position + GetDetectDirection(MouthDetectAngle, m_SupportJoints.HeadCenter);
            tmpRay = new Ray(mouthCastFrom, m_SupportJoints.HeadCenter.position - mouthCastFrom);
            hits = Physics.RaycastAll(ray: tmpRay,
                                      maxDistance: 10f,
                                      layerMask: ~LayerMask.NameToLayer("Face"));
            if (hits.Length > 0)
            {
   
                bool set = false;
                foreach (RaycastHit hit in hits)
                {
                    if (gameObject.IsParentOf(hit.collider.gameObject))
                    {
                        m_SupportJoints.Mouth.position = hit.point;
                        Vector3 forward = hit.normal;
                        Vector3 up = Vector3.Cross(forward, m_SupportJoints.Mouth.right);
                        if (up == Vector3.zero)
                        {
                            up = Vector3.Cross(forward, Vector3.forward);
                        }
                        m_SupportJoints.Mouth.rotation = Quaternion.LookRotation(forward, up.normalized);
                        set = true;
                        break;
                    }
                }
                if (!set)
                {
                    Debug.LogError("Didn't hit face");
                    return Result.Failed;
                }
            }
            else
            {
                Debug.LogError("Didn't hit anything");
                return Result.Failed;
            }
            faceCollider.convex = false;
            #endregion

            return Result.Success;
        }


        [System.Serializable]
        public struct BodyCode
        {
            public static BodyCode Default = new BodyCode(0.75f, 0.75f, 1.15f, 1.15f, 0.3f);
            public static BodyCode Zero = new BodyCode(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
            public float LeftArmLength;
            public float RightArmLength;
            public float LeftLegLength;
            public float RightLegLength;
            public float HipRadious;

            public BodyCode(float leftArmLength, float rightArmLength, float leftLegLength, float rightLegLength, float hipMeasurements)
            {
                LeftArmLength = leftArmLength;
                RightArmLength = rightArmLength;
                LeftLegLength = leftLegLength;
                RightLegLength = rightLegLength;
                HipRadious = hipMeasurements;
            }
        }

        Vector3 GetDetectDirection(Vector3 detectAngle, Transform space)
        {
            Vector3 dir = Quaternion.Euler(detectAngle) * Vector3.forward;
            dir = space.rotation * dir;
            return dir;
        }

        private void OnDrawGizmosSelected()
        {
            if (m_SupportJoints.HeadCenter)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(m_SupportJoints.HeadCenter.position, m_SupportJoints.HeadCenter.position + GetDetectDirection(LeftCheekDetectAngle, m_SupportJoints.HeadCenter));
                Gizmos.DrawLine(m_SupportJoints.HeadCenter.position, m_SupportJoints.HeadCenter.position + GetDetectDirection(RightCheekDetectAngle, m_SupportJoints.HeadCenter));
                Gizmos.DrawLine(m_SupportJoints.HeadCenter.position, m_SupportJoints.HeadCenter.position + GetDetectDirection(MouthDetectAngle, m_SupportJoints.HeadCenter));
            }
        }


        [System.Serializable]
        public class SupportJoints
        {
            public Transform HeadCenter;
            public Transform LeftCheek;
            public Transform RightCheek;
            public Transform Mouth;

            public SupportJoints(Transform head)
            {
                HeadCenter = new GameObject("HeadCenter").transform;
                LeftCheek = new GameObject("LeftCheek").transform;
                RightCheek = new GameObject("RightCheek").transform;
                Mouth = new GameObject("Mouth").transform;
                aszHoldable headHoldable = HeadCenter.gameObject.AddComponent<aszHoldable>();
                Mouth.gameObject.AddComponent<aszKissable>();
                headHoldable.HoldTrans[0] = RightCheek;
                headHoldable.HoldTrans[1] = LeftCheek;
                HeadCenter.parent = head;
                LeftCheek.parent = head;
                RightCheek.parent = head;
                Mouth.parent = head;
                HeadCenter.position = head.position;
                LeftCheek.position = head.position;
                RightCheek.position = head.position;
                Mouth.position = head.position;
                HeadCenter.rotation = head.rotation;
                LeftCheek.rotation = head.rotation;
                RightCheek.rotation = head.rotation;
                Mouth.rotation = head.rotation;
            }
        }
    }
}

