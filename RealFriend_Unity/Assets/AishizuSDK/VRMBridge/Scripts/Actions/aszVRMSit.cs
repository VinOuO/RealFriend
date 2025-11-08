using Aishizu.UnityCore;
using Aishizu.Native;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMSit : aszVRMAction
    {
        public override string ToString() => $"SitAction(TargetId={TargetId})";
        private aszSitable m_Sitable; public aszSitable Sitable => m_Sitable;
        public override void OnStart()
        {
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszSitable sitable)
            {
                return;
            }
            m_Sitable = sitable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }
            aszVRMActor.SitOnObject(this, undo: false);
        }

        public override void OnEnd()
        {
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszSitable sitable)
            {
                return;
            }
            m_Sitable = sitable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }
            aszVRMActor.SitOnObject(this, undo: true);
        }
    }
}
