using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.UnityCore.Actions
{
    public class aszUnityActionKiss : aszIUnityPrimitiveAction
    {
        public string Name => "Kiss";
        public GameObject TargetObject { get; }
        public aszIAction SourceAction { get; }
        public bool IsValid => Verify();


        public aszUnityActionKiss(aszActionKiss source)
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
        public override string ToString() => $"KissAction(TargetId={TargetObject})";
    }
}
