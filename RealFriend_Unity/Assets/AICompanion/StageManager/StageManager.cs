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
        aszScriptManager.Instance.InterableManager.AddInterable(m_DummyBodyInfo.GetSupportJoints.Mouth.GetComponent<aszKissable>());
        aszScriptManager.Instance.InterableManager.AddInterable(m_DummyBodyInfo.GetSupportJoints.HeadCenter.GetComponent<aszHoldable>());
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
        Task<Result> setupTask = aszScriptManager.Instance.SetUpStage();
        while (true)
        {
            #region SetUpStage
            yield return new WaitUntil(() => setupTask.IsCompleted);
            Result result = setupTask.Result;
            if (setupTask.Result != Result.Success)
            {
                Debug.Log("Fail SetUpStage");
                yield break;
            }

            Task<Result> updateTask = aszScriptManager.Instance.UpdateStage();
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
            #region PlayerInput
            PlayerInputUIController.Instance.StartSevice();
            yield return new WaitUntil(() => PlayerInputUIController.Instance.FinishedPlayerInput);
            string playerInput = PlayerInputUIController.Instance.GetPlayerInput();
            PlayerInputUIController.Instance.EndSevice();
            Debug.Log(playerInput);
            #endregion
        }
    }

}
