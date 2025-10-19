using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Aishizu.Native;

namespace Aishizu.UnityCore
{
    public class AIMediator : MonoBehaviour
    {
        public static AIMediator Instance;

        [Header("Setting")]
        public string ModelName;
        [Range(0.3f, 2.0f)]
        public float Temperature = 0.65f;

        [Header("Prompt")]
        [TextArea(3, 10)]
        public string InitPrompt = "";
        [TextArea(3, 10)]
        public string UserPrompt = "";
        public List<string> ArousalHintPrompts;


        [Header("Debug")]
        [SerializeField]
        public AIResponse CurrentResponse;
        public float CurrentArousal;

        void OnEnable()
        {
            Instance = this;
        }


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

            messages.Add(new AIMessage("system", InitPrompt));
            messages.Add(new AIMessage("user", UserPrompt));
            string tmp;
            if (GetArousalHintPrompt(out tmp) == Result.Success)
            {
                messages.Add(new AIMessage("user", tmp));
            }

            StartCoroutine(SendingMessages(messages));
        }

        Result GetArousalHintPrompt(out string result)
        {
            if (ArousalHintPrompts.Count < 4)
            {
                result = "";
                return Result.Failed;
            }
            if (!AIActor.Instance.CurrentBehavior.IsValid)
            {
                result = "";
                return Result.Failed;
            }
            if (AIActor.Instance.CurrentBehavior.Arousal < 0.2f)
            {
                result = ArousalHintPrompts[0];
                return Result.Success;
            }
            else if (AIActor.Instance.CurrentBehavior.Arousal < 0.2f)
            {
                result = ArousalHintPrompts[1];
                return Result.Success;
            }
            else if (AIActor.Instance.CurrentBehavior.Arousal < 0.2f)
            {
                result = ArousalHintPrompts[2];
                return Result.Success;
            }
            else
            {
                result = ArousalHintPrompts[3];
                return Result.Success;
            }
        }

        IEnumerator SendingMessages(List<AIMessage> messages)
        {
            string apiUrl = "http://localhost:1234/v1/chat/completions";

            string jsonPayload = JsonUtility.ToJson(new AIRequest
            {
                model = ModelName,
                messages = messages,
                max_tokens = 200,
                temperature = Temperature
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
                Debug.Log("[AI] RevicedResponse: " + CurrentResponse.choices[0].message.content);
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

        [System.Serializable]
        public class AIBehavior
        {
            public string response;
            public string action;
            public string scenario;
            public string target;
            public string arousal;
        }
    }
}

