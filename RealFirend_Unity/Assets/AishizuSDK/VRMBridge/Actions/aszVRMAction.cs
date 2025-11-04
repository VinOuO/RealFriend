using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.VRMBridge
{
    public class aszVRMAction : aszAction
    {
        public virtual void OnStart()
        {
            Debug.Log("Start");
        }

        public virtual void OnFinish()
        {
            Debug.Log("Finish");
        }
    }
}
