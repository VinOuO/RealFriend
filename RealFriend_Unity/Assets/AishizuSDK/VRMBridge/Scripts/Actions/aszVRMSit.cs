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
            base.OnStart();
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (target is not aszSitable sitable)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            m_Sitable = sitable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            aszVRMActor.SitOnObject(this, undo: false);
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (target is not aszSitable sitable)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            m_Sitable = sitable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            aszVRMActor.SitOnObject(this, undo: true);
        }
    }
}
