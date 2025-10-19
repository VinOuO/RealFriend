using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.UnityCore;
using Aishizu.Native;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMSit : aszAction
    {
        public override string ToString() => $"SitAction(TargetId={TargetId})";
        private aszSitable m_Sitable; public aszSitable Sitable => m_Sitable;

        protected override void OnStart()
        {
            if (aszInterableManager.Instance.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (target is not aszSitable sitable)
            {
                SetFinish(Result.Failed);
                return;
            }
            m_Sitable = sitable;
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

            aszVRMActor.SitOnObject(this);
        }
    }
}
