using UnityEngine;

namespace Aishizu.VRMBridge.Actions
{ 
    public class aszVRMTouch : aszVRMAction
    {
        public HumanBodyBones Hand { get; }

        public override string ToString() => $"TouchAction(TargetId={TargetId}, Hand={Hand})";
        private aszTouchable m_Touchable; public aszTouchable Touchable => m_Touchable;

        public override void OnStart()
        {
            /*
            if (aszScriptWritter.Instance.InterableManager.GetInterable(TargetId, out aszInteractable target) != Result.Success)
            {
                return;
            }
            if (target is not aszTouchable touchable)
            {
                return;
            }
            m_Touchable = touchable;
            if (aszScriptWritter.Instance.ActorManager.GetActor(ActorId, out aszActor actor) != Result.Success)
            {
                return;
            }
            if (actor is not aszVRMCharacter aszVRMActor)
            {
                return;
            }

            aszVRMActor.TouchObject(this, undo: State == aszActionState.Running);
            */
        }
    }
}
