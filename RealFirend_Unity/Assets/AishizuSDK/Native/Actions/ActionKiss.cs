namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to kiss a target character or object.
    /// </summary>
    public class ActionKiss : IPrimitiveAction
    {
        public string Name => "Kiss";
        public string TargetId { get; }

        public ActionKiss(string targetId)
        {
            TargetId = targetId;
        }

        public override string ToString() => $"KissAction(TargetId={TargetId})";
    }
}
