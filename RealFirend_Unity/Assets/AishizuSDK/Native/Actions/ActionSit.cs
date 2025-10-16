namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to sit on a sittable object.
    /// </summary>
    public class ActionSit : IPrimitiveAction
    {
        public string Name => "Sit";
        public string TargetId { get; }

        public ActionSit(string targetId)
        {
            TargetId = targetId;
        }

        public override string ToString() => $"SitAction(TargetId={TargetId})";
    }
}
