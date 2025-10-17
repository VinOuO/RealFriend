using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.UnityCore.Actions
{
    public class aszUnityActionSit : aszIUnityPrimitiveAction
    {
        public string Name => "Sit";
        public GameObject TargetObject { get; }
        public aszIAction SourceAction { get; }
        public bool IsValid => Verify();
        public aszUnityActionSit(aszActionSit source)
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

        public override string ToString() => $"SitAction(TargetId={TargetObject})";
    }
}
