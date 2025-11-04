using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.UnityCore;
using Aishizu.Native;
using UnityEditor;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMSit : aszVRMAction
    {
        public override string ToString() => $"SitAction(TargetId={TargetId})";
        private aszSitable m_Sitable; public aszSitable Sitable => m_Sitable;
        public override void OnStart()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszSitable sitable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Sitable = sitable;
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
            aszVRMActor.SitOnObject(this, undo: State == aszActionState.Running);
        }

        public override void OnFinish()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetState(aszActionState.Failed);
                return;
            }
            if (target is not aszSitable sitable)
            {
                SetState(aszActionState.Failed);
                return;
            }
            m_Sitable = sitable;
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
            aszVRMActor.SitOnObject(this, undo: State == aszActionState.Running);
        }
    }
}
