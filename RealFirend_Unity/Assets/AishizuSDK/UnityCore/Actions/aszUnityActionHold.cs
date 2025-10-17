using UnityEngine;
using Aishizu.Native.Actions;


namespace Aishizu.UnityCore.Actions
{
    public class aszUnityActionHold : aszIUnityPrimitiveAction
    {
        public string Name => "Hold";
        public GameObject TargetObject { get; }
        public aszIAction SourceAction { get; }
        public bool IsValid => Verify();

        public aszUnityActionHold(aszActionHold source)
        {
            SourceAction = source;
            TargetObject = GameObject.Find(source.TargetId);
        }
        private bool Verify()
        {
            if (!TargetObject)
            {
                return false;
            }
            return true;
        }

        public override string ToString() => $"HoldAction(TargetId={TargetObject})";
    }
}
