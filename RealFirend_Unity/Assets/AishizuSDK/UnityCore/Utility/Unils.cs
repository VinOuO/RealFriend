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
}
