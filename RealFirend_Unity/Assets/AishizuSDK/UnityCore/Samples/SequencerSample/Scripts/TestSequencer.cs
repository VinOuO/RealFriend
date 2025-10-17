using UnityEngine;
using System.Collections.Generic;
using Aishizu.Native.Actions;
using Aishizu.UnityCore.Sequencer;

public class TestSequencer : MonoBehaviour
{
    [SerializeField] private aszUnitySequencer m_Sequencer;

    [Header("Test JSON Inputs")]
    [TextArea(3, 6)]
    public List<string> jsonList = new()
    {
        @"{ ""actions"": [ { ""type"": ""Walk"", ""targetX"": 1, ""targetY"": 0, ""targetZ"": 2, ""stopDistance"": 0.3 } ] }",
        @"{ ""actions"": [ { ""type"": ""Walk"", ""targetX"": 3, ""targetY"": 0, ""targetZ"": 4, ""stopDistance"": 0.5 } ] }"
    };



    [ContextMenu("PlaySequencer")]
    void PlaySequencer()
    {
        string json = jsonList[0];
        List<aszIAction> actions = aszActionTranslator.TranslateFromJson(json);

        m_Sequencer.Stop();
        m_Sequencer.Enqueue(actions);
        m_Sequencer.Play();
    }
}
