using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMKiss : aszAction
    {
        public override string ToString() => $"KissAction(TargetId={TargetId})";

        private aszKissable m_Kissable; public aszKissable Kissable => m_Kissable;
        protected override void OnStart()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszKissable kissable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Kissable = kissable;
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

            aszVRMActor.KissObject(this, undo: State == aszActionState.Running);
        }

        protected override void OnFinish()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszKissable kissable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Kissable = kissable;
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

            aszVRMActor.KissObject(this, undo: State == aszActionState.Running);
        }
    }
}
