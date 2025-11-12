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
            base.OnStart();
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (target is not aszKissable kissable)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            m_Kissable = kissable;
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

            aszVRMActor.KissObject(this, undo: false);
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (target is not aszKissable kissable)
            {
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            m_Kissable = kissable;
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

            aszVRMActor.KissObject(this, undo: true);
        }
    }
}
