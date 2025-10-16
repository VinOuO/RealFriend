using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class FriendSpeachController : MonoBehaviour
{
    [SerializeField] private string m_InputSpeach; public string InputText => m_InputSpeach;
    [SerializeField] private string m_CurrentSpeach;
    [SerializeField] private List<string> m_PhressedSpeach;
    [SerializeField] private bool m_FinishedSpeaking = true; public bool FinishedSpeaking => m_FinishedSpeaking;
    [SerializeField] private int CurrentWordIndex = 0;
    [SerializeField] private int CurrentCharacterIndex = 0;
    [SerializeField] private float m_CurrentWorldStartTime = 0;
    [SerializeField] private float m_SpeachPeriod = 0.1f; public float SpeachPeriod { get { return m_SpeachPeriod;} set { m_SpeachPeriod = value; } }
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
        YieldInstruction wait = new WaitForSeconds(m_SpeachPeriod);
        m_FinishedSpeaking = false;
        m_PhressedSpeach = m_InputSpeach.Split(new char[] { ' ' }).ToList();
        for (CurrentWordIndex = 0; CurrentWordIndex < m_PhressedSpeach.Count; CurrentWordIndex++)
        {
            m_CurrentWorldStartTime = Time.time;    
            for (CurrentCharacterIndex = 0; CurrentCharacterIndex < m_PhressedSpeach[CurrentWordIndex].Length; CurrentCharacterIndex++)
            {
                m_CurrentSpeach = GetCurrentSpeach();
            }
            yield return wait;
        }
        m_FinishedSpeaking = true;
    }

    public Result GetCurrentMouthShape(out MouthShape result)
    {
        string currentWord = GetCurrentWord();
        currentWord.Insert(0, " "); //---Don't work
        currentWord = " " + currentWord; //---Work
        List<VowelPosition> vowelPositions = new List<VowelPosition>();

        Debug.Log("currentWord: " + currentWord);
        for (int i = 0; i < currentWord.Length; i++)
        {
            Debug.Log("currentChar: " + currentWord[i]);
            if (currentWord[i] == ' ' || currentWord[i].IsVowel())
            {
                Debug.Log("currentVowel: " + currentWord[i] + " , Position: " + ((float)i) / ((float)(currentWord.Length + 1)));
                vowelPositions.Add(new VowelPosition(currentWord[i], ((float)i) / ((float)(currentWord.Length + 1))));
            }
        }
        float woldWholeTime = m_SpeachPeriod * currentWord.Length;
        float currentPosition = (Time.time - m_CurrentWorldStartTime) / woldWholeTime;

        for(int i = 0; i < vowelPositions.Count; i++)
        {
            Debug.Log("vowel" + i + " Position: " + vowelPositions[i].Position);
            Debug.Log("current" + " Position: " + currentPosition);
            if (vowelPositions[i].Position > currentPosition)
            {
                float totalPeriod = vowelPositions[i].Position - vowelPositions[i - 1].Position;
                float currentPeriod = currentPosition - vowelPositions[i - 1].Position;
                result =  new MouthShape(vowelPositions[i - 1].Vowel, vowelPositions[i].Vowel, currentPeriod / totalPeriod, 1 - (currentPeriod / totalPeriod));
                return Result.Success;
            }
        }
        result = MouthShape.Default;
        return Result.Failed;
    }

    public string GetCurrentWord()
    {
        return m_PhressedSpeach[CurrentWordIndex];
    }

    public string GetCurrentSpeach()
    {
        string result = "";
        for(int i = 0; i < CurrentWordIndex - 1; i++)
        {
            result += m_PhressedSpeach[i];
            result += " ";
        }
        result += m_PhressedSpeach[CurrentWordIndex].Substring(0, CurrentCharacterIndex);
        return result;
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
            result = vowel.ToVowelExpression();
            return Result.Success;
        }
        else
        {
            result = FacialExpression.NoVowel;
            return Result.Failed;
        }
    } 

    private struct VowelPosition
    {
        public char Vowel;
        public float Position;

        public VowelPosition(char vowel, float position)
        {
            Vowel = vowel;
            Position = position;
        }
    }
}
