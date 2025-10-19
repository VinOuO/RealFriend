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
            if (aszInterableManager.Instance.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (target is not aszHugable hugable)
            {
                SetFinish(Result.Failed);
                return;
            }
            m_Hugable = hugable;
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

            aszVRMActor.HugObject(this);
        }
    }
}
