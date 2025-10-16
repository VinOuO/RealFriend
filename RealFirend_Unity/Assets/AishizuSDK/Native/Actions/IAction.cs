using System.Collections.Generic;

namespace Aishizu.Native.Actions
{
    public interface IAction
    {
        /// <summary>
        /// Name of the Action
        /// </summary>
        string Name { get; }
    }

    public interface IPrimitiveAction : IAction { }

    public interface ICompositeAction : IAction
    {
        List<IAction> SubActions { get; }
    }
}