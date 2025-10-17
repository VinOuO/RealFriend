using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.UnityCore
{
    public class aszUnityActionWalk : aszIUnityPrimitiveAction
    {
        public string Name { get; }
        public GameObject TargetObject { get; }
        public aszIAction SourceAction { get; }

        /// <summary>Distance threshold to stop walking (meters).</summary>
        public float StopDistance { get; }
        public bool IsValid => Verify();
        public aszUnityActionWalk(aszActionWalk source)
        {
            SourceAction = source;
            TargetObject = GameObject.Find(source.TargetId);
            StopDistance = source.StopDistance;
        }
        private bool Verify()
        {
            if (!TargetObject)
            {
                return false;
            }
            return true;
        }
        public override string ToString() => $"WalkAction(TargetId={TargetObject}, StopDistance={StopDistance})";

    }
}
