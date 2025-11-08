using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMKiss : aszVRMAction
    {
        public override string ToString() => $"KissAction(TargetId={TargetId})";

        private aszKissable m_Kissable; public aszKissable Kissable => m_Kissable;
        public override void OnStart()
        {
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszKissable kissable)
            {
                return;
            }
            m_Kissable = kissable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }

            aszVRMActor.KissObject(this, undo: false);
        }

        public override void OnEnd()
        {
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszKissable kissable)
            {
                return;
            }
            m_Kissable = kissable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }

            aszVRMActor.KissObject(this, undo: true);
        }
    }
}
