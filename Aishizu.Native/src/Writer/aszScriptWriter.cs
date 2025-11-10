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
        public int MaxRetries { get; set; } = 3;
        public bool UseJsonSchemaResponseFormat { get; set; } = true;
        public bool ValidateJsonResponse { get; set; } = true;
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
            prompt.AppendLine("Output Requirements:");
            prompt.AppendLine("- Respond with a single JSON object using UTF-8 encoding.");
            prompt.AppendLine("- The object MUST contain the top-level keys: Actors, Targets, Actions, Events.");
            prompt.AppendLine("- Always emit an array for each key. Use an empty array when there is no data instead of omitting the property.");
            prompt.AppendLine("- Actors and Targets must reference IDs that exist in the catalogs above.");
            prompt.AppendLine("- Actions must include at minimum: ActorId, TargetId, ActionName, IsValid. Additional fields are allowed when defined in the schema.");
            prompt.AppendLine("- Events must include Type and any required fields for that type (ActionId for ActionBegin/ActionEnd, Duration for Wait, etc.).");
            prompt.AppendLine("- Do NOT wrap the JSON in markdown code fences or add commentary.");
            prompt.AppendLine();
            prompt.AppendLine("Example Response:");
            const string exampleJson = @"{
  \"Actors\": [
    { \"Id\": 0, \"Name\": \"Friend\", \"Description\": \"Primary VRM companion\" }
  ],
  \"Targets\": [
    { \"Id\": 0, \"Name\": \"Player\" }
  ],
  \"Actions\": [
    {
      \"ActorId\": 0,
      \"TargetId\": -1,
      \"ActionName\": \"Dialogue\",
      \"Text\": \"Hello there!\",
      \"IsValid\": true
    }
  ],
  \"Events\": [
    { \"Type\": \"ActionBegin\", \"ActionId\": 0 },
    { \"Type\": \"Wait\", \"Duration\": 1.5 },
    { \"Type\": \"ActionEnd\", \"ActionId\": 0 }
  ]
}";
            prompt.AppendLine(exampleJson);
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
            if (string.IsNullOrWhiteSpace(userPrompt) && string.IsNullOrWhiteSpace(systemPrompt))
            {
                return new PromptResult(false, "", "No prompt provided");
            }

            var messages = new List<Dictionary<string, string>>();
            if (!string.IsNullOrWhiteSpace(systemPrompt))
            {
                messages.Add(new Dictionary<string, string>
                {
                    { "role", "system" },
                    { "content", systemPrompt }
                });
            }

            if (!string.IsNullOrWhiteSpace(userPrompt))
            {
                messages.Add(new Dictionary<string, string>
                {
                    { "role", "user" },
                    { "content", userPrompt }
                });
            }

            if (messages.Count == 0)
            {
                return new PromptResult(false, "", "No messages to send");
            }

            var requestData = new Dictionary<string, object?>
            {
                { "model", Model },
                { "temperature", Temperature },
                { "messages", messages }
            };

            if (UseJsonSchemaResponseFormat)
            {
                requestData["response_format"] = BuildResponseFormatPayload();
            }

            string jsonPayload = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            aszLogger.WriteLine("SystemPrompt: \n" + systemPrompt);
            if (!string.IsNullOrWhiteSpace(userPrompt))
            {
                aszLogger.WriteLine("UserPrompt: \n" + userPrompt);
            }

            Exception? lastException = null;
            int attempts = Math.Max(1, MaxRetries);

            for (int attempt = 0; attempt < attempts; attempt++)
            {
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
                    {
                        Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                    };

                    using var response = await m_httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    string responseText = await response.Content.ReadAsStringAsync();
                    PromptResult parsedResult = BuildPromptResultFromResponse(responseText);
                    if (parsedResult.IsValid)
                    {
                        return parsedResult;
                    }

                    lastException = new Exception(parsedResult.Error);
                    aszLogger.WriteLine($"[aszScriptWriter] Failed to parse LLM response: {parsedResult.Error}");
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    aszLogger.WriteLine($"[aszScriptWriter] Attempt {attempt + 1} failed: {ex.Message}");
                }

                if (attempt < attempts - 1)
                {
                    int delayMs = 500 * (attempt + 1);
                    await Task.Delay(delayMs);
                }
            }

            string finalError = lastException?.Message ?? "Unknown error";
            return new PromptResult(false, "", finalError);
        }

        private object BuildResponseFormatPayload()
        {
            return new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "asz_sequence",
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        required = new[] { "Actors", "Targets", "Actions", "Events" },
                        properties = new Dictionary<string, object>
                        {
                            {
                                "Actors",
                                new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        required = new[] { "Id", "Name" },
                                        properties = new Dictionary<string, object>
                                        {
                                            { "Id", new { type = "integer" } },
                                            { "Name", new { type = "string" } },
                                            { "Description", new { type = "string" } }
                                        },
                                        additionalProperties = true
                                    }
                                }
                            },
                            {
                                "Targets",
                                new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        required = new[] { "Id", "Name" },
                                        properties = new Dictionary<string, object>
                                        {
                                            { "Id", new { type = "integer" } },
                                            { "Name", new { type = "string" } },
                                            { "Description", new { type = "string" } }
                                        },
                                        additionalProperties = true
                                    }
                                }
                            },
                            {
                                "Actions",
                                new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        required = new[] { "ActorId", "TargetId", "ActionName", "IsValid" },
                                        properties = new Dictionary<string, object>
                                        {
                                            { "ActorId", new { type = "integer" } },
                                            { "TargetId", new { type = "integer" } },
                                            { "ActionName", new { type = "string" } },
                                            { "IsValid", new { type = "boolean" } },
                                            { "State", new { type = "string" } }
                                        },
                                        additionalProperties = true
                                    }
                                }
                            },
                            {
                                "Events",
                                new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        required = new[] { "Type" },
                                        properties = new Dictionary<string, object>
                                        {
                                            { "Type", new { type = "string" } },
                                            { "ActionId", new { type = "integer" } },
                                            { "ActorId", new { type = "integer" } },
                                            { "Emotion", new { type = "string" } },
                                            { "Duration", new { type = "number" } },
                                            { "Text", new { type = "string" } }
                                        },
                                        additionalProperties = true
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private PromptResult BuildPromptResultFromResponse(string responseText)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(responseText);
                if (document.RootElement.TryGetProperty("choices", out JsonElement choicesElement) && choicesElement.ValueKind == JsonValueKind.Array && choicesElement.GetArrayLength() > 0)
                {
                    JsonElement choice = choicesElement[0];
                    if (choice.TryGetProperty("message", out JsonElement messageElement) && messageElement.TryGetProperty("content", out JsonElement contentElement))
                    {
                        string? content = contentElement.GetString();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            return new PromptResult(false, "", "Empty content returned by LLM");
                        }

                        if (ValidateJsonResponse)
                        {
                            try
                            {
                                JsonDocument.Parse(content);
                            }
                            catch (JsonException ex)
                            {
                                return new PromptResult(false, "", $"Invalid JSON content: {ex.Message}");
                            }
                        }

                        return new PromptResult(true, content, "");
                    }
                }

                if (ValidateJsonResponse)
                {
                    try
                    {
                        JsonDocument.Parse(responseText);
                    }
                    catch (JsonException ex)
                    {
                        return new PromptResult(false, "", $"Invalid JSON response: {ex.Message}");
                    }
                }

                return new PromptResult(true, responseText, "");
            }
            catch (JsonException)
            {
                if (ValidateJsonResponse)
                {
                    try
                    {
                        JsonDocument.Parse(responseText);
                    }
                    catch (JsonException ex)
                    {
                        return new PromptResult(false, "", $"Invalid JSON response: {ex.Message}");
                    }
                }

                return new PromptResult(true, responseText, "");
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
    { ""Type"": ""Wait"", ""Duration"": 1.5 },

    { ""Type"": ""ActionBegin"", ""ActionId"": 5 },
    { ""Type"": ""Wait"", ""Duration"": 3.5 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 5 },
    { ""Type"": ""Wait"", ""Duration"": 1.5 },
    { ""Type"": ""ActionEnd"", ""ActionId"": 1 },

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
