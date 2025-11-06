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

        public void EndAction(aszAction action)
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

        public void de_Dialogue(aszAction action)
        {
            if(action is not aszDialogue dialogue)
            {
                return;
            }
            aszTheater.Instance.ActorManager.GetActor(dialogue.ActorId, out aszActor actor);
            Debug.Log($"{actor.name}:  {dialogue.Text}");
            dialogue.SetState(aszActionState.Succeed);
        }
    }
}
