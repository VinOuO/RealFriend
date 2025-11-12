using Aishizu.UnityCore;
using Aishizu.VRMBridge;
using Aishizu.VRMBridge.Actions;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Aishizu.Native;

public class StageManager : MonoBehaviour
{
    [SerializeField] private aszVRMBodyInfo m_DummyBodyInfo;
    private void Start()
    {
        RegisterActions();
        AddInterables();
        StartCoroutine(Acting());
    }

    #region Init
    private void AddInterables()
    {
        if (m_DummyBodyInfo.GetSupportJoints.Mouth.GetComponent<aszKissable>() == null)
        {
            return;
        }
        aszScriptManager.Instance.InterableManager.AddInterable(
            new aszInteractableInfo(
                interactable: m_DummyBodyInfo.GetSupportJoints.Mouth.GetComponent<aszKissable>(),
                name: $"{m_DummyBodyInfo.name}_mouth",
                discription: "The mouth of the player character."
                )
            );
        aszScriptManager.Instance.InterableManager.AddInterable(
            new aszInteractableInfo(
                interactable: m_DummyBodyInfo.GetSupportJoints.HeadCenter.GetComponent<aszHoldable>(),
                name: $"{m_DummyBodyInfo.name}_face",
                discription: "The face of the player character."
                )
            );
        aszScriptManager.Instance.RegisterInterableList();
    }
    private void RegisterActions()
    {
        Debug.Log(aszScriptManager.Instance == null ? "null" : "not null");
        aszScriptManager.Instance.RegisterAction<aszVRMHold>();
        aszScriptManager.Instance.RegisterAction<aszVRMHug>();
        aszScriptManager.Instance.RegisterAction<aszVRMKiss>();
        aszScriptManager.Instance.RegisterAction<aszVRMReach>();
        aszScriptManager.Instance.RegisterAction<aszVRMSit>();
        aszScriptManager.Instance.RegisterAction<aszVRMTouch>();
        aszScriptManager.Instance.RegisterAction<aszVRMWalk>();
    }
    #endregion

    IEnumerator Acting()
    {
        /*
        Task<Result> setupTask = aszScriptManager.Instance.SetUpStage();
        #region SetUpStage
        yield return new WaitUntil(() => setupTask.IsCompleted);
        Result result = setupTask.Result;
        if (setupTask.Result != Result.Success)
        {
            Debug.Log("Fail SetUpStage");
            yield break;
        }
        #endregion
        */
        while (true)
        {
            #region PlayerInput
            PlayerInputUIController.Instance.StartSevice();
            yield return new WaitUntil(() => PlayerInputUIController.Instance.FinishedPlayerInput);
            string playerInput = PlayerInputUIController.Instance.GetPlayerInput();
            PlayerInputUIController.Instance.EndSevice();
            Debug.Log(playerInput);
            #endregion
            #region UpdateStage
            Task<Result> updateTask = aszScriptManager.Instance.UpdateStage(playerInput);
            yield return new WaitUntil(() => updateTask.IsCompleted);
            if (updateTask.Result != Result.Success)
            {
                Debug.Log("Fail UpdateStage");
                yield break;
            }
            #endregion
            #region RunScript
            bool scriptFinished = false;
            ScriptDirector.Instance.RunScript(aszScriptManager.Instance.GetScript(), () => scriptFinished = true);
            #endregion
            yield return new WaitUntil(() => scriptFinished);
        }
    }

}
