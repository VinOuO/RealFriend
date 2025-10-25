using System.Runtime.Serialization;

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
}
