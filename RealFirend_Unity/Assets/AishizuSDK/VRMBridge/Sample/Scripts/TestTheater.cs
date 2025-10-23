using Aishizu.UnityCore;
using Aishizu.VRMBridge.Actions;
using UnityEngine;

namespace Aishizu.VRMBridge
{
    public class TestTheater : MonoBehaviour
    {
        [ContextMenu("RegisterActions")]
        private void RegisterActions()
        {
            aszTheater.Instance.RegisterAction<aszVRMHold>();
            aszTheater.Instance.RegisterAction<aszVRMHug>();
            aszTheater.Instance.RegisterAction<aszVRMKiss>();
            aszTheater.Instance.RegisterAction<aszVRMReach>();
            aszTheater.Instance.RegisterAction<aszVRMSit>();
            aszTheater.Instance.RegisterAction<aszVRMTouch>();
            aszTheater.Instance.RegisterAction<aszVRMWalk>();
        }

    }
}
