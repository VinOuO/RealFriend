namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to hold or pick up a target object.
    /// </summary>
    public class ActionHold : IPrimitiveAction
    {
        public string Name => "Hold";

        public string TargetId { get; }

        public ActionHold(string targetId)
        {
            TargetId = targetId;
        }

        public override string ToString() => $"HoldAction(TargetId={TargetId})";
    }
}