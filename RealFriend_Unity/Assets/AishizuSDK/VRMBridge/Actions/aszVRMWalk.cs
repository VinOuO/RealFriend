using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.UnityCore;
using Aishizu.Native;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMWalk : aszVRMAction
    {
        /// <summary>Distance threshold to stop walking (meters).</summary>
        public float StopDistance { get; }
        public override string ToString() => $"WalkAction(TargetId={TargetId}, StopDistance={StopDistance})";
        private aszWalkable m_Walkable; public aszWalkable Walkable => m_Walkable;

        public override void OnStart()
        {
            /*
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszWalkable walkable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Walkable = walkable;
            if (aszTheater.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                SetState(aszActionState.Failed);
                return;
            }

            aszVRMActor.WalkToTarget(this, undo: State == aszActionState.Running);
            */
        }

    }
}
