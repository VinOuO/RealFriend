using UnityEngine;
using Aishizu.Native;

namespace Aishizu.UnityCore
{
    public static class aszVector3Extensions
    {
        public static Vector3 ToUnity(this aszVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static aszVector3 Toasz(this Vector3 v)
        {
            return new aszVector3(v.x, v.y, v.z);
        }
    }
}