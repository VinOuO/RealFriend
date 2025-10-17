using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.UnityCore.Actions
{
    public class aszUnityActionReach : aszIUnityPrimitiveAction
    {
        public string Name => "Reach";
        public GameObject TargetObject { get; }
        public aszIAction SourceAction { get; }
        public HumanBodyBones Hand { get; }
        public bool IsValid => Verify();
        public aszUnityActionReach(aszActionReach source)
        {
            SourceAction = source;
            TargetObject = GameObject.Find(source.TargetId);
            Hand = source.Hand.ToLower().Contains("lefthand") ? HumanBodyBones.LeftHand : (source.Hand.ToLower().Contains("righthand") ? HumanBodyBones.RightHand : HumanBodyBones.Hips);
        }

        private bool Verify()
        {
            if (!TargetObject)
            {
                return false;
            }
            if (Hand != HumanBodyBones.LeftHand && Hand != HumanBodyBones.RightHand)
            {
                return false;
            }
            return true;
        }



        public override string ToString() => $"ReachAction(TargetId={TargetObject}, Hand={Hand})";
    }
}
