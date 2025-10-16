namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Represents an intent to hug a target character or object.
    /// </summary>
    public class ActionHug : IPrimitiveAction
    {
        public string Name => "Hug";
        public string TargetId { get; }

        public ActionHug(string targetId)
        {
            TargetId = targetId;
        }

        public override string ToString() => $"HugAction(TargetId={TargetId})";
    }
}