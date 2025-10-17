namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to kiss a target character or object.
    /// </summary>
    public class aszActionKiss : aszIPrimitiveAction
    {
        public string Name => "Kiss";
        public string TargetId { get; }

        public aszActionKiss(string targetId)
        {
            TargetId = targetId;
        }

        public override string ToString() => $"KissAction(TargetId={TargetId})";
    }
}
