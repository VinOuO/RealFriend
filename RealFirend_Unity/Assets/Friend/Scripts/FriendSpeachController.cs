using UnityEngine;
using System.Collections;

public class FriendSpeachController : MonoBehaviour
{
    [SerializeField] private string m_InputText; public string InputText => m_InputText;
    [SerializeField] private string m_CurrentSpeach;
    [SerializeField] private string m_PhressedSpeach;
    [SerializeField] private bool m_FinishedSpeaking = true; public bool FinishedSpeaking => m_FinishedSpeaking;
    private int Progress = 0;
    private void OnEnable()
    {
        
    }

    [ContextMenu("Speak")]
    public void Speak()
    {
        StartCoroutine(Speaking());
    }

    private IEnumerator Speaking()
    {
        YieldInstruction wait = new WaitForSeconds(0.1f);
        m_FinishedSpeaking = false;
        for (int progress = 0; progress <= m_InputText.Length; progress++)
        {
            Progress = progress;
            m_CurrentSpeach = m_InputText.Substring(0, progress);
            yield return wait;
        }
        m_FinishedSpeaking = true;
    }

    private Result FindCloestVowel(string speach, out char result)
    {
        char[] tmp = speach.ToCharArray();
        for (int i = tmp.Length - 1; i >= 0; i--)
        {
            if (tmp[i].IsVowel())
            {
                result = tmp[i];
                return Result.Success;
            }
        }
        result = 'x';
        return Result.Failed;
    }

    public Result GetCurrentVowelExpression(out FacialExpression result)
    {
        if(FindCloestVowel(m_CurrentSpeach, out char vowel) == Result.Success)
        {
            result = vowel.ToVowel();
            return Result.Success;
        }
        else
        {
            result = FacialExpression.NoVowel;
            return Result.Failed;
        }
    } 
}
