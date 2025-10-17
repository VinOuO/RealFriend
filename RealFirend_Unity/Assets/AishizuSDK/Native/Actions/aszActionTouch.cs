namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to touch or interact with a target object.
    /// </summary>
    public class aszActionTouch : aszIPrimitiveAction
    {
        public string Name => "Touch";

        public string TargetId { get; }
        public string Hand { get; }

        public aszActionTouch(string targetId, string hand = "Right")
        {
            TargetId = targetId;
            Hand = hand;
        }

        public override string ToString() => $"TouchAction(TargetId={TargetId}, Hand={Hand})";
    }
}
