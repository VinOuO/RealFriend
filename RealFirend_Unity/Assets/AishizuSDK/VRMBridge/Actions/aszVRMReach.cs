using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMReach : aszAction
    {
        public HumanBodyBones Hand { get; }
        public override string ToString() => $"ReachAction(TargetId={TargetId}, Hand={Hand})";
        private aszReachable m_Reachable; public aszReachable Reachable => m_Reachable;

        protected override void OnStart()
        {
            if (aszInterableManager.Instance.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (target is not aszReachable reachable)
            {
                SetFinish(Result.Failed);
                return;
            }
            m_Reachable = reachable;
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

            aszVRMActor.ReachObject(this);
        }
    }
}
