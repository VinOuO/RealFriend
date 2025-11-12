using UnityEngine;
using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMHug : aszVRMAction
    {
        public override string ToString() => $"HugAction(TargetId={TargetId})";

        private aszHugable m_Hugable; public aszHugable Hugable => m_Hugable;
        public override void OnStart()
        {
            base.OnStart();
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (target is not aszHugable hugable)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            m_Hugable = hugable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }

            aszVRMActor.HugObject(this, undo: false);
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (target is not aszHugable hugable)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            m_Hugable = hugable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                m_Stage = aszVRMActionStage.Failed;
                m_Stage = aszVRMActionStage.Failed;
                return;
            }

            aszVRMActor.HugObject(this, undo: true);
        }
    }
}
