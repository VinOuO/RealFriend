using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.UnityCore;
using Aishizu.Native;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMWalk : aszAction
    {
        /// <summary>Distance threshold to stop walking (meters).</summary>
        public float StopDistance { get; }
        public override string ToString() => $"WalkAction(TargetId={TargetId}, StopDistance={StopDistance})";
        private aszWalkable m_Walkable; public aszWalkable Walkable => m_Walkable;

        protected override void OnStart()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (target is not aszWalkable walkable)
            {
                SetFinish(Result.Failed);
                return;
            }
            m_Walkable = walkable;
            if (aszTheater.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                SetFinish(Result.Failed);
                return;
            }

            aszVRMActor.WalkToTarget(this);
        }

    }
}
