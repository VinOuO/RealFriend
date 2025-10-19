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
            if (aszInterableManager.Instance.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (target is not aszKissable kissable)
            {
                SetFinish(Result.Failed);
                return;
            }
            m_Kissable = kissable;
            if (aszActorManager.Instance.GetActor(ActorId, out aszCharacter actor) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (actor is not aszVRMCharacterController aszVRMActor)
            {
                SetFinish(Result.Failed);
                return;
            }

            aszVRMActor.KissObject(this);
        }
    }
}
