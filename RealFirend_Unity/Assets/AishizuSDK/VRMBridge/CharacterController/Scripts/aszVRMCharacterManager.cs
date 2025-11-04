using Aishizu.Native;
using Aishizu.Native.Actions;
using Aishizu.Native.Events;
using Aishizu.UnityCore;
using UnityEngine;
using UnityEngine.Events;

namespace Aishizu.VRMBridge
{
    public class aszVRMCharacterManager : MonoBehaviour
    {
        public void StartAction(aszAction action)
        {
            if(action is not aszVRMAction vrmAction)
            {
                return;
            }

            vrmAction.OnStart();
        }

        public void FinishAction(aszAction action)
        {
            if (action is not aszVRMAction vrmAction)
            {
                return;
            }

            vrmAction.OnFinish();
        }

        public void de_Emotion(aszEmotionChange emotionChange)
        {
            aszTheater.Instance.ActorManager.GetActor(emotionChange.ActorId, out aszActor actor);
            Debug.Log($"Character: {actor.name} is {emotionChange.Emotion} for {emotionChange.Duration} sec");
        }
    }
}
