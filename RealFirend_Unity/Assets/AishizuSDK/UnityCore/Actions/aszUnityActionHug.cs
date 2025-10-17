using UnityEngine;
using Aishizu.Native.Actions;
using UnityEngine.XR;

namespace Aishizu.UnityCore.Actions
{
    public class aszUnityActionHug : aszIUnityPrimitiveAction
    {
        public string Name => "Hug";
        public GameObject TargetObject { get; }
        public aszIAction SourceAction { get; }
        public bool IsValid => Verify();
        public aszUnityActionHug(aszActionHug source)
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

        public override string ToString() => $"HugAction(TargetId={TargetObject})";
    }
}
