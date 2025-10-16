using System;
using UnityEngine;
using static AIMediator;
using UniHumanoid;
using RootMotion.FinalIK;
using UniVRM10;
using UnityEditor;

public static class AIExt
{
    public static AIResponse ToAIResponse(this string value)
    {
        AIResponse result = JsonUtility.FromJson<AIResponse>(value);

        return result;
    }

    public static AIBehavior ToAIBehavior(this string value)
    {
        AIBehavior result = JsonUtility.FromJson<AIBehavior>(value);
        return result;
    }

    public static AIActor.ActorBehavior ToActorBehavior(this string value)
    {
        return value.ToAIBehavior().ToActorBehavior();
    }

    public static AIActor.ActorBehavior ToActorBehavior(this AIBehavior value)
    {
        AIActor.ActorBehavior result = new AIActor.ActorBehavior();
        object tmp;
        if(!Enum.TryParse(typeof(AIActor.BehaviorAction), value.action, out tmp))
        {
            result.IsValid = false;
        }
        else
        {
            result.Action = (AIActor.BehaviorAction)tmp;
        }

        if (!Enum.TryParse(typeof(AIActor.BehaviorTarget), value.target, out tmp))
        {
            result.IsValid = false;
        }
        else
        {
            result.Target = (AIActor.BehaviorTarget)tmp;
        }
        int tmpArousal = 0;
        if (!Int32.TryParse(value.arousal, out tmpArousal))
        {
            result.IsValid = false;
        }
        else
        {
            result.Arousal = ((float)tmpArousal)/100f;
        }
        result.Response = value.response;
        result.Scenario = value.scenario;
        return result;
    }
}

public static class Vector3Ext
{
    public static Vector3 ToLevelPosition(this Vector3 self)
    {
        return new Vector3(self.x, 0, self.z);
    }

    public static Vector3 ToLevelPosition(this Vector3 self, Transform levelRef)
    {
        return new Vector3(self.x, levelRef.position.y, self.z);
    }
}

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
        if(result == null)
        {
            return Result.Failed;
        }
        return Result.Success;
    }
}

public static class MouthShapeExt
{
    public static FacialBlend ToFacialBlend(this MouthShape self)
    {
        return new FacialBlend(self.Vowel1.ToVowelExpression(), self.Vowel2.ToVowelExpression(), self.Weight1, self.Weight2);
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
        if(self.target == null)
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
        if( m_FullBodyBipedIK.solver.leftShoulderEffector == self   ||
            m_FullBodyBipedIK.solver.leftFootEffector == self       ||
            m_FullBodyBipedIK.solver.leftHandEffector == self       ||
            m_FullBodyBipedIK.solver.leftThighEffector == self      )
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

public static class GameObjectExt
{
    public static GameObject FindInChild(this GameObject self, string name) 
    {
        for (int i = 0; i < self.transform.childCount; i++)
        {
            if(self.transform.GetChild(i).name == name)
            {
                return self.transform.GetChild(i).gameObject;
            }
            else
            {
                GameObject tmp = self.transform.GetChild(i).gameObject.FindInChild(name);
                if (tmp != null)
                {
                    return tmp;
                }
            }
        }
        return null;
    }

    public static bool IsParentOf(this GameObject self, GameObject testChild)
    {
        for (int i = 0; i < self.transform.childCount; i++)
        {
            if(testChild == self.transform.GetChild(i).gameObject)
            {
                return true;
            }
            else if(self.transform.GetChild(i).gameObject.IsParentOf(testChild))
            {
                return true;
            }
        }
        return false;
    }
}

public static class SupportJointsExt
{
    public static void Clear(this BodyInfo.SupportJoints self)
    {
        if(self == null)
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
        self = null;
    }
}

public static class Vrm10RuntimeExpressionExt
{
    public static void CleanExpression(this Vrm10RuntimeExpression self)
    {
        foreach(ExpressionKey key in self.ExpressionKeys)
        {
            self.SetWeight(key, 0);
        }
    } 
}

public static class charExt
{
    public static bool IsVowel(this char self)
    {
        if( self == 'A' |
            self == 'a' |
            self == 'E' |
            self == 'e' |
            self == 'I' |
            self == 'i' |
            self == 'O' |
            self == 'o' |
            self == 'U' |
            self == 'u' )
        {
            return true;
        }
        return false;
    }

    public static FacialExpression ToVowelExpression(this char self)
    {
        if (self == 'A' |
            self == 'a' )
        {
            return FacialExpression.A;
        }
        if (self == 'E' |
            self == 'e' )
        {
            return FacialExpression.E;
        }
        if (self == 'I' |
            self == 'i' )
        {
            return FacialExpression.I;
        }
        if (self == 'O' |
            self == 'o' )
        {
            return FacialExpression.O;
        }
        if (self == 'U' |
            self == 'u' )
        {
            return FacialExpression.U;
        }
        return FacialExpression.NoVowel;
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


    /// <returns>If failed, return FacialExpression.Null</returns>
    public static Result ExtractExpression(this FacialExpression self, out FacialExpression result)
    {
        foreach (FacialExpression expression in Enum.GetValues(typeof(FacialExpression)))
        {
            if (self.HasFlag(expression) && (FacialExpression.Natural   | 
                                             FacialExpression.Angry     | 
                                             FacialExpression.Happy     | 
                                             FacialExpression.Sad       | 
                                             FacialExpression.Surprised ).HasFlag(expression))
            {
                result = expression;
                return Result.Success;
            }
        }
        result = FacialExpression.Null;
        return Result.Failed;
    }

    /// <returns>If failed, return FacialExpression.Null</returns>
    public static Result ExtractVowel(this FacialExpression self, out FacialExpression result)
    {
        // All vowel flags
        const FacialExpression VowelMask =
            FacialExpression.A | FacialExpression.E |
            FacialExpression.I | FacialExpression.O | FacialExpression.U;

        // Bitwise AND to find if self contains any vowel
        FacialExpression matched = self & VowelMask;

        if (matched != 0)
        {
            // Find the first vowel it contains
            foreach (FacialExpression vowel in new[]{
                                                    FacialExpression.A,
                                                    FacialExpression.E,
                                                    FacialExpression.I,
                                                    FacialExpression.O,
                                                    FacialExpression.U,
                                                    })
            {
                if ((matched & vowel) != 0)
                {
                    result = vowel;
                    return Result.Success;
                }
            }
        }

        result = FacialExpression.Null;
        return Result.Failed;
    }
}