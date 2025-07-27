using UnityEngine;
using UniHumanoid;
using UniVRM10;
using UnityEngine.EventSystems;
using JetBrains.Annotations;
using Unity.VisualScripting;
using static BodyInfo;
using UnityEngine.XR;

public class BodyInfo : MonoBehaviour
{
    [Header("Configeration")]
    [SerializeField] private Humanoid m_Humanoid;

    [Header("Infos")]
    [SerializeField] private LimbLength m_LimbLength = LimbLength.Default; public LimbLength GetLimbLength => m_LimbLength;
    [SerializeField] private SupportJoints m_SupportJoints; public SupportJoints GetSupportJoints => m_SupportJoints;

    [Header("Debug")]
    public Vector3 LeftCheekDetectAngle;
    public Vector3 RightCheekDetectAngle;

    private void OnEnable()
    {
        m_Humanoid = GetComponent<Humanoid>();
        UpdateLimbInfo();
        SetSupportJoints();
    }

    void UpdateLimbInfo()
    {
        m_LimbLength = LimbLength.Zero;
        m_LimbLength.LeftArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm);
        m_LimbLength.LeftArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);

        m_LimbLength.RightArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm);
        m_LimbLength.RightArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);

        m_LimbLength.LeftLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg);
        m_LimbLength.LeftLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);

        m_LimbLength.RightLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg);
        m_LimbLength.RightLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);
    }

    [ContextMenu("SetSupportJoints")]
    public void de_SetSupportJoints()
    {
        Debug.Log(SetSupportJoints());
    }
    Result SetSupportJoints()
    {
        Debug.Log("SetSupportJoints");
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

        if(!faceCollider || !faceRenderer)
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
            foreach(RaycastHit hit in hits)
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


        return Result.Success;
    }


    [System.Serializable]
    public struct LimbLength
    {
        public static LimbLength Default = new LimbLength(0.75f, 0.75f, 1.15f, 1.15f);
        public static LimbLength Zero = new LimbLength(0.0f, 0.0f, 0.0f, 0.0f);
        public float LeftArmLength;
        public float RightArmLength;
        public float LeftLegLength;
        public float RightLegLength;

        public LimbLength(float leftArmLength, float rightArmLength, float leftLegLength, float rightLegLength)
        {
            LeftArmLength = leftArmLength;
            RightArmLength = rightArmLength;
            LeftLegLength = leftLegLength;
            RightLegLength = rightLegLength;
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
        }
    }


    [System.Serializable]
    public class SupportJoints
    {
        public Transform HeadCenter;
        public Transform LeftCheek;
        public Transform RightCheek;

        public SupportJoints(Transform head)
        {
            HeadCenter = new GameObject("HeadCenter").transform;
            LeftCheek = new GameObject("LeftCheek").transform;
            RightCheek = new GameObject("RightCheek").transform;
            HeadCenter.parent = head;
            LeftCheek.parent = head;
            RightCheek.parent = head;
            HeadCenter.position = head.position;
            LeftCheek.position = head.position;
            RightCheek.position = head.position;
            HeadCenter.rotation = head.rotation;
            LeftCheek.rotation = head.rotation;
            RightCheek.rotation = head.rotation;
        }
    }
}
