using Aishizu.Native.Actions;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aishizu.Native.Services
{
    public class aszAIMediator
    {
        private readonly HttpClient m_httpClient = new();
        private readonly aszActorService m_ActorService; public aszActorService ActorService => m_ActorService;
        private readonly aszActionService m_ActionService; public aszActionService ActionService => m_ActionService;
        private readonly aszTargetService m_TargetService; public aszTargetService TargetService => m_TargetService;

        public string Endpoint { get; set; } = "http://localhost:1234/v1/chat/completions";
        public string Model { get; set; } = "gpt-4o-mini";
        public float Temperature { get; set; } = 0.65f;

        public aszAIMediator()
        {
            m_ActorService = new aszActorService();
            m_ActionService = new aszActionService();
            m_TargetService = new aszTargetService();
        }

        public aszAIMediator(
            aszActorService actorService,
            aszActionService actionService,
            aszTargetService targetService)
        {
            m_ActorService = actorService;
            m_ActionService = actionService;
            m_TargetService = targetService;
        }

        /// <summary>
        /// Generates the system prompt that provides the AI with contextual data.
        /// </summary>
        public string InitSystemPrompt()
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
        public async Task<string> SendPromptAsync(string userPrompt)
        {
            string systemPrompt = InitSystemPrompt();

            var requestData = new
            {
                model = Model,
                temperature = Temperature,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
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
                return responseText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIMediator] Error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
