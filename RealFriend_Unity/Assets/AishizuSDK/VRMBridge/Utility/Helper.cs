using System;
using UnityEngine;
using UniHumanoid;
using RootMotion.FinalIK;
using UniVRM10;
using Aishizu.UnityCore;
using Aishizu.Native;

namespace Aishizu.VRMBridge
{
    public static class HumanoidExt
    {
        public static Vector3 GetJointPosition(this Humanoid self, HumanBodyBones joint)
        {
            return self.GetBoneTransform(joint).position;
        }

        public static float GetDistanceBetweenJoints(this Humanoid self, HumanBodyBones joint1, HumanBodyBones joint2)
        {
            return Vector3.Distance(self.GetBoneTransform(joint1).position, self.GetBoneTransform(joint2).position);
        }

        public static Result GetComponentFromJoint<T>(this Humanoid self, HumanBodyBones joint, out T result)
        {
            result = self.GetBoneTransform(joint).GetComponent<T>();
            if (result == null)
            {
                return Result.Failed;
            }
            return Result.Success;
        }
    }

    public static class aszInteractableExt
    {
        public static bool IsVRMCharacter(this aszInteractable self)
        {
            if (self.InteractTransform.GetComponent<aszVRMBodyInfo>())
            {
                return true;
            }
            return false;
        }

        public static Result GetVRMBodyInfo(this aszInteractable self, out aszVRMBodyInfo result)
        {
            result = self.InteractTransform.GetComponent<aszVRMBodyInfo>();
            if (result)
            {
                return Result.Success;
            }
            return Result.Failed;
        }
    }

    public static class aszHeadEffectorExt
    {
        public static void SetBendWeight(this FBBIKHeadEffector self, float weight)
        {
            self.bendWeight = weight;
            self.CCDWeight = weight;
        }
    }

    public static class IKEffectorExt
    {
        public static Quaternion RotationOffset(this IKEffector self, Vector3 upAxis, FullBodyBipedIK m_FullBodyBipedIK)
        {
            return Quaternion.AngleAxis(angle: self.IsLeftBody(m_FullBodyBipedIK) ? 90f : -90f,
                                 axis: upAxis);
        }

        /// <summary>
        /// Set the API target with an rotation offset based on m_FullBodyBipedIK 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <param name="m_FullBodyBipedIK">Rotation offset helper</param>
        /// <returns></returns>
        public static Result VRMSetTarget(this IKEffector self, Transform target, FullBodyBipedIK m_FullBodyBipedIK)
        {
            if (self.target == null)
            {
                return Result.Failed;
            }

            self.target.SetParent(target);
            self.target.localPosition = Vector3.zero;
            self.target.localRotation = self.RotationOffset(Vector3.up, m_FullBodyBipedIK) * Quaternion.identity;
            return Result.Success;
        }

        public static Result VRMSetTarget(this IKEffector self, Transform target)
        {
            if (self.target == null)
            {
                return Result.Failed;
            }

            self.target.SetParent(target);
            self.target.localPosition = Vector3.zero;
            self.target.localRotation = Quaternion.identity;
            return Result.Success;
        }

        public static bool IsLeftBody(this IKEffector self, FullBodyBipedIK m_FullBodyBipedIK)
        {
            if (m_FullBodyBipedIK.solver.leftShoulderEffector == self ||
                m_FullBodyBipedIK.solver.leftFootEffector == self ||
                m_FullBodyBipedIK.solver.leftHandEffector == self ||
                m_FullBodyBipedIK.solver.leftThighEffector == self)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static class FBBIKHeadEffectorExt
    {
        public static Result VRMSetTarget(this FBBIKHeadEffector self, Transform target)
        {
            self.transform.SetParent(target);
            self.transform.localPosition = Vector3.zero;
            self.transform.localRotation = Quaternion.identity;
            return Result.Success;
        }
    }

    public static class SupportJointsExt
    {
        public static void Clear(this aszVRMBodyInfo.SupportJoints self)
        {
            if (self == null)
            {
                return;
            }
            if (self.HeadCenter)
            {
                UnityEngine.Object.DestroyImmediate(self.HeadCenter.gameObject);
            }
            if (self.LeftCheek)
            {
                UnityEngine.Object.DestroyImmediate(self.LeftCheek.gameObject);
            }
            if (self.RightCheek)
            {
                UnityEngine.Object.DestroyImmediate(self.RightCheek.gameObject);
            }
            if (self.Mouth)
            {
                UnityEngine.Object.DestroyImmediate(self.Mouth.gameObject);
            }
            self = null;
        }
    }

    public static class Vrm10RuntimeExpressionExt
    {
        public static void CleanExpression(this Vrm10RuntimeExpression self)
        {
            foreach (ExpressionKey key in self.ExpressionKeys)
            {
                self.SetWeight(key, 0);
            }
        }
    }
    public static class FacialExpressionExt
    {
        public static ExpressionKey ToVRMExpressionKey(this FacialExpression self)
        {
            switch (self)
            {
                case FacialExpression.Natural:
                    return ExpressionKey.Neutral;
                case FacialExpression.Happy:
                    return ExpressionKey.Happy;
                case FacialExpression.Sad:
                    return ExpressionKey.Sad;
                case FacialExpression.Angry:
                    return ExpressionKey.Angry;
                case FacialExpression.Surprised:
                    return ExpressionKey.Surprised;

                case FacialExpression.A:
                    return ExpressionKey.Aa;
                case FacialExpression.E:
                    return ExpressionKey.Ee;
                case FacialExpression.I:
                    return ExpressionKey.Ih;
                case FacialExpression.O:
                    return ExpressionKey.Oh;
                case FacialExpression.U:
                    return ExpressionKey.Ou;
            }
            return ExpressionKey.Neutral;
        }
    }
}


