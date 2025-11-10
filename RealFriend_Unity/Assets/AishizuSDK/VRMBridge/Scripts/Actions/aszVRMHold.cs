using Aishizu.Native;
using Aishizu.UnityCore;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMHold : aszVRMAction
    {
        public override string ToString() => $"HoldAction(TargetId={TargetId})";

        private aszHoldable m_Holdable; public aszHoldable Holdable => m_Holdable;
        public override void OnStart()
        {
            base.OnStart();
            if(aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszHoldable holdable)
            {
                return;
            }
            m_Holdable = holdable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }
            aszVRMActor.HoldObject(this, undo: false);
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (aszScriptManager.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszHoldable holdable)
            {
                return;
            }
            m_Holdable = holdable;
            if (aszScriptManager.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }
            aszVRMActor.HoldObject(this, undo: true);
        }
    }
}
