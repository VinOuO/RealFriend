using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMHug : aszAction
    {
        public override string ToString() => $"HugAction(TargetId={TargetId})";

        private aszHugable m_Hugable; public aszHugable Hugable => m_Hugable;
        protected override void OnStart()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszHugable hugable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Hugable = hugable;
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

            aszVRMActor.HugObject(this, undo: State == aszActionState.Running);
        }

        protected override void OnFinish()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszHugable hugable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Hugable = hugable;
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

            aszVRMActor.HugObject(this, undo: State == aszActionState.Running);
        }
    }
}
