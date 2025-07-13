using System;
using Unity.VisualScripting;
using UnityEngine;
using static AIMediator;

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
    public static Vector2 ToLevelPosition(this Vector3 self)
    {
        return new Vector2(self.x, self.z);
    }
}