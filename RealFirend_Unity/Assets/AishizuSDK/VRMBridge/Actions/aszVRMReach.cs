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
            /*
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
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

            aszVRMActor.ReachObject(this);
            */
        }
    }
}
