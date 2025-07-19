using UnityEngine;
using UniHumanoid;
public class FriendBodyInfo : MonoBehaviour
{
    public float LeftArmLength = 0;
    public float RightArmLength = 0;
    public float LeftLegLength = 0;
    public float RightLegLength = 0;
    [SerializeField] Humanoid m_Humanoid;

    private void OnEnable()
    {
        m_Humanoid = GetComponent<Humanoid>();
        UpdateLimbInfo();
    }

    void UpdateLimbInfo()
    {
        LeftArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm);
        LeftArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);

        RightArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm);
        RightArmLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);

        LeftLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg);
        LeftLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);

        RightLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg);
        RightLegLength += m_Humanoid.GetDistanceBetweenJoints(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);
    }

    
}
