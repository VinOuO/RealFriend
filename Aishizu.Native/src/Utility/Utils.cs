using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Aishizu.Native
{
    public enum Result
    {
        Unkown = -1,
        Success = 1,
        Failed = 2,
    }
    public struct PromptResult
    {
        public bool IsValid { get; set; }
        public string Response { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public PromptResult(bool isValid, string response, string error)
        {
            IsValid = isValid;
            Response = response;
            Error = error;
        }
    }

    public enum LifeStatus
    {
        RunningSequence,
        WaitingForSceneUpdate,
    }
    public static class aszJsonSettings
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
        };
    }
}
