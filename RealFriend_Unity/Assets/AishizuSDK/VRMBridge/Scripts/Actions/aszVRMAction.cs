using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.VRMBridge.Actions
{
    public class aszVRMAction : aszAction
    {
        protected aszVRMActionStage m_Stage = aszVRMActionStage.Idling; public aszVRMActionStage Stage => m_Stage;
        public void ProgressStage()
        {
            if(m_Stage != aszVRMActionStage.Ended)
            {
                m_Stage++;
            }
        }
        public virtual void OnStart()
        {
            ProgressStage();
        }

        public virtual void OnEnd()
        {
            ProgressStage();
        }

        public enum aszVRMActionStage
        {
            Idling = 1,
            Starting = 2,
            Running = 3,
            Ending = 4,
            Ended = 5,
            Failed = -1,
        }
    }
}
