using UnityEngine;
using System.Collections.Generic;

namespace Aishizu.UnityCore
{
    /// <summary>
    /// Provides reusable yield instructions to prevent GC allocations in coroutines.
    /// </summary>
    public static class aszUnityCoroutine
    {
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        private static readonly Dictionary<float, WaitForSeconds> _waits = new();

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (!_waits.TryGetValue(seconds, out var w))
            {
                w = new WaitForSeconds(seconds);
                _waits.Add(seconds, w);
            }
            return w;
        }
    }

    public struct MouthShape
    {
        public char Vowel1, Vowel2;
        public float Weight1, Weight2;
        public static MouthShape Default = new MouthShape(' ', ' ', 0, 0);
        public MouthShape(char vowel1, char vowel2, float weight1, float weight2)
        {
            Vowel1 = vowel1;
            Vowel2 = vowel2;
            Weight1 = weight1;
            Weight2 = weight2;
        }
    }

    public struct FacialBlend
    {
        public FacialExpression Expression1, Expression2;
        public float Weight1, Weight2;
        public static FacialBlend Default = new FacialBlend(FacialExpression.Natural, FacialExpression.Natural, 0, 0);
        public FacialBlend(FacialExpression expression1, FacialExpression expression2, float weight1, float weight2)
        {
            Expression1 = expression1;
            Expression2 = expression2;
            Weight1 = weight1;
            Weight2 = weight2;
        }
    }
}
