namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to hug a target character or object.
    /// </summary>
    public class aszActionHug : aszIPrimitiveAction
    {
        public string Name => "Hug";
        public string TargetId { get; }

        public aszActionHug(string targetId)
        {
            TargetId = targetId;
        }

        public override string ToString() => $"HugAction(TargetId={TargetId})";
    }
}