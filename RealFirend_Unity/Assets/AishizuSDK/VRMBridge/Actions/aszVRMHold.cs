using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.Native;
using Aishizu.UnityCore;
using UnityEditor;

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
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszHoldable holdable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Holdable = holdable;
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
            aszVRMActor.HoldObject(this, undo: State == aszActionState.Running);
        }

        protected override void OnFinish()
        {

            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszHoldable holdable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Holdable = holdable;
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
            aszVRMActor.HoldObject(this, undo: State == aszActionState.Running);
        }
    }
}
