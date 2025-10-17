namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to sit on a sittable object.
    /// </summary>
    public class aszActionSit : aszIPrimitiveAction
    {
        public string Name => "Sit";
        public string TargetId { get; }

        public aszActionSit(string targetId)
        {
            TargetId = targetId;
        }

        public override string ToString() => $"SitAction(TargetId={TargetId})";
    }
}
