using System;
using UnityEngine;
using static AIMediator;
using UniHumanoid;

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