using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Aishizu.Native.Events;
using Aishizu.Native.Actions;
using Aishizu.Native.Services;

namespace Aishizu.Native
{
    public class aszScript
    {
        private List<aszAction> m_ActionPool = new List<aszAction>(); 
        private Queue<aszIEvent> m_Sequence = new();
        public bool IsFinished => m_Sequence.Count <= 0;

        public aszScript(List<aszAction> actionPool, Queue<aszIEvent> sequence) 
        {
            m_ActionPool = actionPool;
            m_Sequence = sequence;
        }

        public Result NextEvent(out aszIEvent? nextEvent)
        {
            if(m_Sequence.Count > 0)
            {
                nextEvent = m_Sequence.Dequeue();
                aszLogger.WriteLine($"[aszScript] Dequeued event: {nextEvent.ToJson()}");
                return Result.Success;
            }
            else
            {
                nextEvent = null;
                return Result.Failed;
            }
        }

        public Result GetAction(int actionId, out aszAction? action)
        {
            if(actionId >= 0 && actionId < m_ActionPool.Count)
            {
                action = m_ActionPool[actionId];
                return Result.Success;
            }
            action = null;
            aszLogger.WriteLine($"[aszScript] actionId is out of actionPool's range, current actionPool count: {m_ActionPool.Count}");
            return Result.Failed;   
        } 
    }

    public class aszScriptWriter
    {
        private readonly HttpClient m_httpClient = new();
        private readonly aszActorService m_ActorService; public aszActorService ActorService => m_ActorService;
        private readonly aszActionService m_ActionService; public aszActionService ActionService => m_ActionService;
        private readonly aszTargetService m_TargetService; public aszTargetService TargetService => m_TargetService;
        private aszSequenceService m_SequenceService; public aszSequenceService SequenceService => m_SequenceService;


        public string Endpoint { get; set; } = "http://localhost:1234/v1/chat/completions";
        public string Model { get; set; } = "gpt-4o-mini";
        public float Temperature { get; set; } = 0.65f;
        private bool m_IsDescribing = false;
        public bool DebugMode = true;
        public aszScriptWriter()
        {
            m_ActorService = new aszActorService();
            m_ActionService = new aszActionService();
            m_TargetService = new aszTargetService();
            m_SequenceService = new aszSequenceService();
        }

        /// <summary>
        /// Generates the system prompt that provides the AI with contextual data.
        /// </summary>
        private string InitSystemPrompt()
        {
            var prompt = new StringBuilder();
            prompt.AppendLine("You are an AI that controls character behavior.");
            prompt.AppendLine("You can issue actions to characters using JSON format.");
            prompt.AppendLine();
            prompt.AppendLine("=== Actors ===");
            prompt.AppendLine(m_ActorService.ToJson());
            prompt.AppendLine();
            prompt.AppendLine("=== Targets ===");
            prompt.AppendLine(m_TargetService.ToJson());
            prompt.AppendLine();
            prompt.AppendLine("=== Actions ===");
            prompt.AppendLine(m_ActionService.GetActionSchemasAsJson());
            prompt.AppendLine();
            prompt.AppendLine("When responding, always output a JSON sequence following these action definitions.");
            return prompt.ToString();
        }

        /// <summary>
        /// Sends a user prompt + world context to the AI endpoint and retrieves a structured response.
        /// </summary>
        private async Task<PromptResult> SendPromptAsync(string userPrompt = "", string systemPrompt = "")
        {
            if (DebugMode)
            {
                return new PromptResult(true, "DebugMode", "DebugMode");
            }
            if(userPrompt == "" && systemPrompt == "")
            {
                return new PromptResult(false, "", "");
            }

            aszLogger.WriteLine("SystemPrompt: \n" + systemPrompt);
            var requestData = new
            {
                model = Model,
                temperature = Temperature,
                messages =
                userPrompt != "" && systemPrompt != "" ?    new[] 
                                                            {
                                                                new { role = "system", Content = systemPrompt},
                                                                new { role = "user", Content = userPrompt}
                                                            }
                : userPrompt != "" ?                        new[]
                                                            {
                                                                new { role = "user", Content = userPrompt}
                                                            }
                :                                           new[]
                                                            {
                                                                new { role = "system", Content = systemPrompt},
                                                            }


            };

            string jsonPayload = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                using var response = await m_httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();
                return new PromptResult(true, responseText, "");
            }
            catch (Exception ex)
            {
                aszLogger.WriteLine($"[aszScriptWriter] Error: {ex.Message}");
                return new PromptResult(false, "", ex.Message);
            }
        }
        #region CallFlow
        public async Task<Result> SetUpScene()
        {
            PromptResult result = await SendPromptAsync(systemPrompt: InitSystemPrompt());
            return result.IsValid ? Result.Success : Result.Failed;
        }
        string mockData = @"
{
  ""Actors"": [
    { ""Id"": 0, ""Name"": ""Friend1"", ""Description"": ""The main VRM character"" }
  ],
  ""Targets"": [
    { ""Id"": 0, ""Name"": ""Hugable"" },
    { ""Id"": 1, ""Name"": ""Cube"" },
    { ""Id"": 2, ""Name"": ""Mouth"" },
    { ""Id"": 3, ""Name"": ""HeadCenter"" }
  ],
  ""Actions"": [
    {
      ""ActorId"": 0,
      ""TargetId"": 1,
      ""ActionName"": ""VRMSit"",
      ""IsValid"": true,
      ""State"": ""Idle"",
      ""IsFinished"": false
    },
    {
      ""ActorId"": 0,
      ""TargetId"": 0,
      ""ActionName"": ""VRMHug"",
      ""IsValid"": true,
      ""State"": ""Idle"",
      ""IsFinished"": false
    },
    {
      ""ActorId"": 0,
      ""TargetId"": 3,
      ""ActionName"": ""VRMHold"",
      ""IsValid"": true,
      ""State"": ""Idle"",
      ""IsFinished"": false
    },
    {
      ""ActorId"": 0,
      ""TargetId"": 2,
      ""ActionName"": ""VRMKiss"",
      ""IsValid"": true,
      ""State"": ""Idle"",
      ""IsFinished"": false
    },
    {
      ""ActorId"": 0,
      ""TargetId"": -1,
      ""ActionName"": ""Dialogue"",
      ""Text"": ""Hey, I missed you."",
      ""IsValid"": true,
      ""State"": ""Idle"",
      ""IsFinished"": false
    },
    {
      ""ActorId"": 0,
      ""TargetId"": -1,
      ""ActionName"": ""Dialogue"",
      ""Text"": ""You're warm... stay like this for a bit."",
      ""IsValid"": true,
      ""State"": ""Idle"",
      ""IsFinished"": false
    },
    {
      ""ActorId"": 0,
      ""TargetId"": -1,
      ""ActionName"": ""Dialogue"",
      ""Text"": ""Heh... that was nice."",
      ""IsValid"": true,
      ""State"": ""Idle"",
      ""IsFinished"": false
    }
  ],
  ""Events"": [
    { ""Type"": ""EmotionChange"", ""ActorId"": 0, ""Emotion"": ""Natural"", ""Duration"": 1.5 },

    { ""Type"": ""ActionBegin"", ""ActionId"": 4 },
    { ""Type"": ""Wait"", ""Duration"": 3.0 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 4 },

    { ""Type"": ""ActionBegin"", ""ActionId"": 1 },
    { ""Type"": ""EmotionChange"", ""ActorId"": 0, ""Emotion"": ""Happy"", ""Duration"": 2.5 },
    { ""Type"": ""Wait"", ""Duration"": 2.5 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 1 },

    { ""Type"": ""ActionBegin"", ""ActionId"": 5 },
    { ""Type"": ""Wait"", ""Duration"": 3.5 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 5 },

    { ""Type"": ""ActionBegin"", ""ActionId"": 2 },
    { ""Type"": ""Wait"", ""Duration"": 1.0 },
    { ""Type"": ""ActionBegin"", ""ActionId"": 3 },
    { ""Type"": ""Wait"", ""Duration"": 5.0 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 3 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 2 },

    { ""Type"": ""ActionBegin"", ""ActionId"": 6 },
    { ""Type"": ""Wait"", ""Duration"": 2.0 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 6 },

    { ""Type"": ""ActionBegin"", ""ActionId"": 0 },
    { ""Type"": ""Wait"", ""Duration"": 2.0 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 0 },

    { ""Type"": ""EmotionChange"", ""ActorId"": 0, ""Emotion"": ""Natural"", ""Duration"": 1.5 }
  ]
}";
        public async Task<Result> DescribeCurrentScene()
        {
            if (m_IsDescribing)
            {
                aszLogger.WriteLine("[aszScriptWriter] DescribeCurrentScene skipped — already running.");
                return Result.Failed;
            }
            m_IsDescribing = true;
            try
            {
                PromptResult result = await SendPromptAsync(systemPrompt: "");
                if (DebugMode)
                {
                    result = new PromptResult(true, mockData, "");
                }
                if (result.IsValid)
                {
                    if(m_ActionService.JsonToActions(result.Response, out List<aszAction> actionList) == Result.Success &&
                       m_SequenceService.JsonToSequence(result.Response, out List<aszIEvent> eventList) == Result.Success)
                    {
                        m_SequenceService.GenerateSequence(actionList, eventList);
                        aszLogger.WriteLine("[aszScriptWriter] DescribeCurrentScene Success.");
                        return Result.Success;
                    }
                    else
                    {
                        aszLogger.WriteLine("[aszScriptWriter] DescribeCurrentScene Failed.");
                        return Result.Failed;
                    }
                }
                return result.IsValid ? Result.Success : Result.Failed;
            }
            finally
            {
                m_IsDescribing = false;
            }
        }
        #endregion

        public aszScript GetScript()
        {
            aszScript script = new aszScript(m_SequenceService.Actions, m_SequenceService.Sequence);
            return script;
        }
    }
}
