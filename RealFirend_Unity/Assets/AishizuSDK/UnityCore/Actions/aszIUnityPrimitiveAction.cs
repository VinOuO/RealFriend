using Aishizu.Native.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace Aishizu.UnityCore
{

    public interface aszIUnityAction
    {
        /// <summary>
        /// Name of the Action
        /// </summary>
        string Name { get; }
        aszIAction SourceAction { get; }
        bool IsValid { get; }
    }


    public interface aszIUnityPrimitiveAction : aszIUnityAction 
    {
        GameObject TargetObject { get; }
    }

    public interface aszIUNityCompositeAction : aszIUnityAction
    {
        List<aszIUnityAction> SubActions { get; }
    }
}
