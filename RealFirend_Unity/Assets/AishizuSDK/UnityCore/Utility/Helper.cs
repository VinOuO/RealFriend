using System;
using UnityEngine;
using static Aishizu.UnityCore.AIMediator;

namespace Aishizu.UnityCore
{
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
            if (!Enum.TryParse(typeof(AIActor.BehaviorAction), value.action, out tmp))
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
                result.Arousal = ((float)tmpArousal) / 100f;
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

    public static class charExt
    {
        public static bool IsVowel(this char self)
        {
            if (self == 'A' |
                self == 'a' |
                self == 'E' |
                self == 'e' |
                self == 'I' |
                self == 'i' |
                self == 'O' |
                self == 'o' |
                self == 'U' |
                self == 'u')
            {
                return true;
            }
            return false;
        }

        public static FacialExpression ToVowelExpression(this char self)
        {
            if (self == 'A' |
                self == 'a')
            {
                return FacialExpression.A;
            }
            if (self == 'E' |
                self == 'e')
            {
                return FacialExpression.E;
            }
            if (self == 'I' |
                self == 'i')
            {
                return FacialExpression.I;
            }
            if (self == 'O' |
                self == 'o')
            {
                return FacialExpression.O;
            }
            if (self == 'U' |
                self == 'u')
            {
                return FacialExpression.U;
            }
            return FacialExpression.NoVowel;
        }
    }

    public static class MouthShapeExt
    {
        public static FacialBlend ToFacialBlend(this MouthShape self)
        {
            return new FacialBlend(self.Vowel1.ToVowelExpression(), self.Vowel2.ToVowelExpression(), self.Weight1, self.Weight2);
        }
    }

    public static class GameObjectExt
    {
        public static GameObject FindInChild(this GameObject self, string name)
        {
            for (int i = 0; i < self.transform.childCount; i++)
            {
                if (self.transform.GetChild(i).name == name)
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
                if (testChild == self.transform.GetChild(i).gameObject)
                {
                    return true;
                }
                else if (self.transform.GetChild(i).gameObject.IsParentOf(testChild))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public static class FacialExpressionExt
    {
        /// <returns>If failed, return FacialExpression.Null</returns>
        public static Result ExtractExpression(this FacialExpression self, out FacialExpression result)
        {
            foreach (FacialExpression expression in Enum.GetValues(typeof(FacialExpression)))
            {
                if (self.HasFlag(expression) && (FacialExpression.Natural |
                                                 FacialExpression.Angry |
                                                 FacialExpression.Happy |
                                                 FacialExpression.Sad |
                                                 FacialExpression.Surprised).HasFlag(expression))
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
}
