using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aishizu.Native;
using Aishizu.Native.Events;
using Aishizu.UnityCore;
using Aishizu.VRMBridge.Actions;
using Aishizu.Native.Actions;

namespace Aishizu.VRMBridge
{
    public class aszVRMScriptDirector : MonoBehaviour
    {
        public static aszVRMScriptDirector Instance;

        private void OnEnable()
        {
            Instance = this;
        }

        public IEnumerator RunningScript(aszScript script)
        {
            Debug.Log($"[VRMDirector] Start running Script");
            while (!script.IsFinished)
            {
                if (script.NextEvent(out aszIEvent currentEvent) == Result.Success)
                {
                    switch (currentEvent)
                    {
                        case aszActionStart actionStart:
                            if (script.GetAction(actionStart.actionId, out aszAction actionS) == Result.Success)
                            {
                                Debug.Log($"[VRMDirector] Starting {actionS.ActionName}");
                                if(actionS is aszVRMAction vrmAction)
                                {
                                    vrmAction.OnStart();
                                    while (!vrmAction.IsFinished)
                                    {
                                        yield return aszUnityCoroutine.WaitForEndOfFrame;
                                    }
                                }
                                if(actionS is aszDialogue dialogue)
                                {
                                    if (aszScriptManager.Instance.ActorManager.GetActor(dialogue.ActorId, out aszActor actorD) == Result.Success)
                                    {
                                        Debug.Log($"[VRMDirector] Character [{actorD.name}] says: {dialogue.Text}");
                                        if(actorD is aszVRMCharacter vrmCharacter)
                                        {
                                            vrmCharacter.Speak(dialogue.Text);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log($"[VRMDirector] Unknown ActionStart");
                            }
                            continue;
                        case aszActionEnd actionEnd:
                            if (script.GetAction(actionEnd.actionId, out aszAction actionE) == Result.Success)
                            {
                                Debug.Log($"[VRMDirector] Ending {actionE.ActionName}");
                                if (actionE is aszVRMAction vrmAction)
                                {
                                    vrmAction.OnEnd();
                                    while (!vrmAction.IsFinished)
                                    {
                                        yield return aszUnityCoroutine.WaitForEndOfFrame;
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log($"[VRMDirector] Unknown ActionEnd");
                            }
                            continue;
                        case aszWait wait:
                            while(wait.Duration > 0)
                            {
                                yield return aszUnityCoroutine.WaitForEndOfFrame;
                                wait.Duration -= Time.deltaTime;
                            }
                            continue;
                        case aszEmotionChange emotionChange:
                            if (aszScriptManager.Instance.ActorManager.GetActor(emotionChange.ActorId, out aszActor actorE) == Result.Success)
                            {
                                Debug.Log($"[VRMDirector] Character [{actorE.name}] feels: {emotionChange.Emotion}");
                                if(actorE is aszVRMCharacter vrmCharacter)
                                {
                                    vrmCharacter.VRMAnimationController.CleanEmotionExpression();
                                    vrmCharacter.VRMAnimationController.SetFacialExpression(emotionChange.Emotion.ToFacialExpression(), 1f);
                                }
                            }
                            continue;
                    }
                }
            }
            Debug.Log($"[VRMDirector] Finished running Script");
        }
    }
}