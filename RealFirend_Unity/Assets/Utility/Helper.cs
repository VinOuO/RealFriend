using Unity.VisualScripting;
using UnityEngine;
using static AIClient;

public static class AIClientExt
{
    public static AIResponse ToAIResponse(this string value)
    {
        AIResponse response = JsonUtility.FromJson<AIResponse>(value);

        return response;
    }
}
