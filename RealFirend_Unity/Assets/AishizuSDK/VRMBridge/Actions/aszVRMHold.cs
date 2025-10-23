using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMHold : aszAction
    {
        public override string ToString() => $"HoldAction(TargetId={TargetId})";

        private aszHoldable m_Holdable; public aszHoldable Holdable => m_Holdable;
        protected override void OnStart()
        {
            if(aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (target is not aszHoldable holdable)
            {
                SetFinish(Result.Failed);
                return;
            }
            m_Holdable = holdable;
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

            aszVRMActor.HoldObject(this);
        }
    }
}
