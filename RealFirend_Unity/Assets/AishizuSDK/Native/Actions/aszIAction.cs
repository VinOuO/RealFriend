using System.Collections.Generic;

namespace Aishizu.Native.Actions
{
    public interface aszIAction
    {
        /// <summary>
        /// Name of the Action
        /// </summary>
        string Name { get; }
    }

    public interface aszIPrimitiveAction : aszIAction
    {
        string TargetId { get; }
    }

    public interface aszICompositeAction : aszIAction
    {
        List<aszIAction> SubActions { get; }
    }
}