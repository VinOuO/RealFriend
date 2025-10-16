namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to reach toward a target object with a specific hand.
    /// </summary>
    public class ActionReach : IPrimitiveAction
    {
        public string Name => "Reach";

        public string TargetId { get; }
        public string Hand { get; }

        public ActionReach(string targetId, string hand = "Right")
        {
            TargetId = targetId;
            Hand = hand;
        }

        public override string ToString() => $"ReachAction(TargetId={TargetId}, Hand={Hand})";
    }
}