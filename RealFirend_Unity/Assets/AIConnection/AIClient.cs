using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AIClient : MonoBehaviour
{
    [TextArea(3, 10)]
    public string InitPrompt = "";

    [TextArea(3, 10)]
    public string UserPrompt = "";

    [Header("Debug")]
    [SerializeField]
    AIResponse CurrentResponse;


    [ContextMenu("Init")]
    public void Init()
    {
        List<AIMessage> messages = new List<AIMessage>();
        messages.Add(new AIMessage("system", InitPrompt));

        messages.Add(new AIMessage("user", UserPrompt));
        StartCoroutine(SendingMessages(messages));
    }

    [ContextMenu("Send Prompt")]
    public void SendPrompt()
    {
        List<AIMessage> messages = new List<AIMessage>();
        messages.Add(new AIMessage("user", UserPrompt));

        StartCoroutine(SendingMessages(messages));
    }

    IEnumerator SendingMessages(List<AIMessage> messages)
    {
        string apiUrl = "http://localhost:1234/v1/chat/completions";

        string jsonPayload = JsonUtility.ToJson(new AIRequest
        {
            model = "mythomax-l2-13b.Q4_K_S.gguf",
            messages = messages,
            max_tokens = 200,
            temperature = 0.9f
        });

        Debug.Log(jsonPayload);

        byte[] postData = Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("[AI] Sending prompt...");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[AI] Error: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            CurrentResponse = response.ToAIResponse();
        }
    }


    [System.Serializable]
    public class AIRequest
    {
        public string model;
        public List<AIMessage> messages;
        public int max_tokens;
        public float temperature;
    }

    [System.Serializable]
    public class AIMessage
    {
        public string role;
        public string content;

        public AIMessage(string in_role, string in_content)
        {
            role = in_role;
            content = in_content;   
        }
    }

    [System.Serializable]
    public class AIResponse
    {
        public string model;
        public AIChoice[] choices;

        [System.Serializable]
        public class AIChoice
        {
            public int index;
            public string logprobs;
            public string finish_reason;
            public AIMessage message;
        }
    }

}