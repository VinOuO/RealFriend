namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Virtual "walk" instruction ¡X represents intent to move to a position.
    /// </summary>
    public class aszActionWalk : aszIPrimitiveAction
    {
        public string Name => "Walk";

        /// <summary>Target object name or ID.</summary>
        public string TargetId { get; }

        /// <summary>Distance threshold to stop walking (meters).</summary>
        public float StopDistance { get; }

        public aszActionWalk(string targetId, float stopDistance = 0.5f)
        {
            TargetId = targetId;
            StopDistance = stopDistance;
        }

        public override string ToString()
            => $"WalkAction(TargetId={TargetId}, StopDistance={StopDistance})";
    }
}
