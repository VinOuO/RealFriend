using UnityEngine;
using Aishizu.Native.Actions;
using Aishizu.UnityCore;
using Aishizu.Native;

namespace Aishizu.VRMBridge.Actions 
{ 
    public class aszVRMTouch : aszAction
    {
        public HumanBodyBones Hand { get; }

        public override string ToString() => $"TouchAction(TargetId={TargetId}, Hand={Hand})";
        private aszTouchable m_Touchable; public aszTouchable Touchable => m_Touchable;

        protected override void OnStart()
        {
            if (aszTheater.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                SetFinish(Result.Failed);
                return;
            }
            if (target is not aszTouchable touchable)
            {
                SetFinish(Result.Failed);
                return;
            }
            m_Touchable = touchable;
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

            aszVRMActor.TouchObject(this);
        }
    }
}
