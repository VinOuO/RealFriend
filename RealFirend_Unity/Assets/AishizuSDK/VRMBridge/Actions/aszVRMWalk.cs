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
            if (aszInterableManager.Instance.GetInterable(TargetId, out aszInteractable target) != Result.Success)
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
            if (aszActorManager.Instance.GetActor(ActorId, out aszCharacter actor) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (actor is not aszVRMCharacterController aszVRMActor)
            {
                SetFinish(Result.Failed);
                return;
            }

            aszVRMActor.WalkToTarget(this);
        }

    }
}
