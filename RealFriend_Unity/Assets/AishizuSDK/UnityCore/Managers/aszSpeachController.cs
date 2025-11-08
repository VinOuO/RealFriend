using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aishizu.Native;

namespace Aishizu.UnityCore.Speach
{
    public class aszSpeachController : MonoBehaviour
    {
        [SerializeField] private string m_InputSpeach; public string InputText => m_InputSpeach;
        [SerializeField] private string m_CurrentSpeach;
        [SerializeField] private List<string> m_ProcessedSpeach;
        [SerializeField] private bool m_FinishedSpeaking = true; public bool FinishedSpeaking => m_FinishedSpeaking;
        [SerializeField] private int m_CurrentWordIndex = 0;
        [SerializeField] private int m_CurrentCharacterIndex = 0;
        [SerializeField] private float m_CurrentWorldStartTime = 0;
        [SerializeField] private float m_SpeachPeriod = 0.1f; public float SpeachPeriod { get { return m_SpeachPeriod; } set { m_SpeachPeriod = value; } }
        private void OnEnable()
        {

        }

        public void Speak(string text)
        {
            m_InputSpeach = text;
            StartCoroutine(Speaking());
        }

        private IEnumerator Speaking()
        {
            m_FinishedSpeaking = false;
            m_ProcessedSpeach = m_InputSpeach.Split(new char[] { ' ' }).ToList();
            m_ProcessedSpeach[m_ProcessedSpeach.Count - 1] += " ";
            for (m_CurrentWordIndex = 0; m_CurrentWordIndex < m_ProcessedSpeach.Count; m_CurrentWordIndex++)
            {
                m_CurrentWorldStartTime = Time.time;
                for (m_CurrentCharacterIndex = 0; m_CurrentCharacterIndex < m_ProcessedSpeach[m_CurrentWordIndex].Length; m_CurrentCharacterIndex++)
                {
                    m_CurrentSpeach = GetCurrentSpeach();
                    yield return aszUnityCoroutine.WaitForSeconds(m_SpeachPeriod);
                }
            }
            m_CurrentWordIndex = Mathf.Clamp(m_CurrentWordIndex, 0, m_ProcessedSpeach.Count - 1);
            yield return aszUnityCoroutine.WaitForSeconds(m_SpeachPeriod);
            m_FinishedSpeaking = true;
        }

        public Result GetCurrentMouthShape(out MouthShape result)
        {

            string currentWord = GetCurrentWord();
            currentWord = " " + currentWord;
            List<VowelPosition> vowelPositions = new List<VowelPosition>();
            for (int i = 0; i < currentWord.Length; i++)
            {
                if (currentWord[i] == ' ' || currentWord[i].IsVowel())
                {
                    vowelPositions.Add(new VowelPosition(currentWord[i], ((float)i) / ((float)(currentWord.Length + 1))));
                }
            }
            float wordTotalTime = m_SpeachPeriod * currentWord.Length;
            float currentPosition = (Time.time - m_CurrentWorldStartTime) / wordTotalTime;
            for (int i = 0; i < vowelPositions.Count; i++)
            {
                if (vowelPositions[i].Position > currentPosition)
                {
                    float totalPeriod = vowelPositions[i].Position - vowelPositions[i - 1].Position;
                    float currentPeriod = currentPosition - vowelPositions[i - 1].Position;
                    result = new MouthShape(vowelPositions[i].Vowel, vowelPositions[i - 1].Vowel, (currentPeriod / totalPeriod), 1 - (currentPeriod / totalPeriod));
                    return Result.Success;
                }
            }
            result = MouthShape.Default;
            return Result.Failed;
        }

        public string GetCurrentWord()
        {
            return m_ProcessedSpeach[m_CurrentWordIndex];
        }

        public string GetCurrentSpeach()
        {
            string result = "";
            for (int i = 0; i < m_CurrentWordIndex; i++)
            {
                result += m_ProcessedSpeach[i];
                result += " ";
            }
            result += m_ProcessedSpeach[m_CurrentWordIndex].Substring(0, m_CurrentCharacterIndex);
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
            if (FindCloestVowel(m_CurrentSpeach, out char vowel) == Result.Success)
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
}


