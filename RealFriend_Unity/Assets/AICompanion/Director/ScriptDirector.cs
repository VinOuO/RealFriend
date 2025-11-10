using UnityEngine;
using System.Collections;
using Aishizu.Native;
using Aishizu.Native.Events;
using Aishizu.UnityCore;
using Aishizu.VRMBridge.Actions;
using Aishizu.Native.Actions;
using Aishizu.VRMBridge;
using System;

public class ScriptDirector : MonoBehaviour
{
    public static ScriptDirector Instance;
    private void OnEnable()
    {
        Instance = this;
    }

    public void RunScript(aszScript script, Action callBack)
    {
        Debug.Log("PPAP");
        StartCoroutine(this.CoroutineWithCallback(RunningScript(script), callBack));
    }

    public IEnumerator RunningScript(aszScript script)
    {
        Debug.Log("8787");
        while (!script.IsFinished)
        {
            if (script.NextEvent(out aszIEvent currentEvent) == Result.Success)
            {
                switch (currentEvent)
                {
                    case aszActionStart actionStart:
                        if (script.GetAction(actionStart.actionId, out aszAction actionS) == Result.Success)
                        {
                            Debug.Log($"[ScriptDirector] Starting {actionS.ActionName}");
                            if (actionS is aszVRMAction vrmAction)
                            {
                                vrmAction.OnStart();
                                while (vrmAction.Stage != aszVRMAction.aszVRMActionStage.Running)
                                {
                                    yield return aszUnityCoroutine.WaitForEndOfFrame;
                                }
                            }
                            if (actionS is aszDialogue dialogue)
                            {
                                if (aszScriptManager.Instance.ActorManager.GetActor(dialogue.ActorId, out aszActor actorD) == Result.Success)
                                {
                                    Debug.Log($"[ScriptDirector] Character [{actorD.name}] says: {dialogue.Text}");
                                    #region AICompanion Dialogue
                                    if (actorD is aszVRMCharacter vrmCharacter)
                                    {
                                        DialogueUIController.Instance.SetVisibility(true);
                                        vrmCharacter.Speak(dialogue.Text);
                                        while (!vrmCharacter.SpeachController.FinishedSpeaking)
                                        {
                                            DialogueUIController.Instance.UpdateDialogue(actorD.name, vrmCharacter.SpeachController.GetCurrentSpeach());
                                            yield return aszUnityCoroutine.WaitForEndOfFrame;   
                                        }
                                        yield return aszUnityCoroutine.WaitForSeconds(vrmCharacter.SpeachController.SpeachPeriod * 5);
                                        DialogueUIController.Instance.UpdateDialogue(actorD.name, dialogue.Text);
                                        DialogueUIController.Instance.SetVisibility(false);
                                    }
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            Debug.Log($"[ScriptDirector] Unknown ActionStart");
                        }
                        continue;
                    case aszActionEnd actionEnd:
                        if (script.GetAction(actionEnd.actionId, out aszAction actionE) == Result.Success)
                        {
                            Debug.Log($"[ScriptDirector] Ending {actionE.ActionName}");
                            if (actionE is aszVRMAction vrmAction)
                            {
                                vrmAction.OnEnd();
                                while (vrmAction.Stage != aszVRMAction.aszVRMActionStage.Ended)
                                {
                                    yield return aszUnityCoroutine.WaitForEndOfFrame;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log($"[ScriptDirector] Unknown ActionEnd");
                        }
                        continue;
                    case aszWait wait:
                        while (wait.Duration > 0)
                        {
                            yield return aszUnityCoroutine.WaitForEndOfFrame;
                            wait.Duration -= Time.deltaTime;
                        }
                        continue;
                    case aszEmotionChange emotionChange:
                        if (aszScriptManager.Instance.ActorManager.GetActor(emotionChange.ActorId, out aszActor actorE) == Result.Success)
                        {
                            Debug.Log($"[ScriptDirector] Character [{actorE.name}] feels: {emotionChange.Emotion}");
                            if (actorE is aszVRMCharacter vrmCharacter)
                            {
                                vrmCharacter.VRMAnimationController.CleanEmotionExpression();
                                vrmCharacter.VRMAnimationController.SetFacialExpression(emotionChange.Emotion.ToFacialExpression(), 1f);
                            }
                        }
                        continue;
                }
            }
        }
    }
}