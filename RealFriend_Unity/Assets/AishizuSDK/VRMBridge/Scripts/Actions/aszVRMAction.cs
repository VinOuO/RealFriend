using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMAction : aszAction
    {
        public bool IsFinished { get; private set; } = false;
        public void SetFinished()
        {
            IsFinished = true;
        }
        public virtual void OnStart()
        {
            Debug.Log("Start");
        }

        public virtual void OnEnd()
        {
            Debug.Log("Finish");
        }
    }
}
