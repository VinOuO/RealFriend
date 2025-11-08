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
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszHugable hugable)
            {
                return;
            }
            m_Hugable = hugable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                Debug.Log("4");
                return;
            }

            aszVRMActor.HugObject(this, undo: false);
        }

        public override void OnEnd()
        {
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszHugable hugable)
            {
                return;
            }
            m_Hugable = hugable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }

            aszVRMActor.HugObject(this, undo: true);
        }
    }
}
