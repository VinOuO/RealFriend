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
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
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
            if (aszTheater.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                SetFinish(Result.Failed);
                return;
            }

            aszVRMActor.SitOnObject(this);
        }
    }
}
