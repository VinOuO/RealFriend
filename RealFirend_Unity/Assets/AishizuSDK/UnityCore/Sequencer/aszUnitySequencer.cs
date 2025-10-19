using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aishizu.Native.Actions;
using Aishizu.Native.Sequencer;
using Aishizu.Native;

namespace Aishizu.UnityCore.Sequencer
{
    /// <summary>
    /// Simple sequential executor that runs Aishizu Native actions one after another.
    /// </summary>
    public class aszUnitySequencer : MonoBehaviour, aszISequencer
    {
        private readonly Queue<aszAction> m_Queue = new();
        private bool m_IsRunning;
        private bool m_IsPaused;

        public void Enqueue(aszAction action)
        {
            m_Queue.Enqueue(action);
        }

        public void Enqueue(IEnumerable<aszAction> actions)
        {
            foreach (aszAction action in actions)
            {
                m_Queue.Enqueue(action);
            }
        }

        public void Play()
        {
            StartCoroutine(RunningSequence());
        }
        public void Pause(bool isPaused)
        {
            m_IsPaused = isPaused;
        }

        public void Stop()
        {
            m_Queue.Clear();
        }

        private IEnumerator RunningSequence()
        {
            m_IsRunning = true;

            while (m_Queue.Count > 0)
            {
                aszAction current = m_Queue.Dequeue();
                current.Start();
                while (!current.IsFinished)
                {
                    current.Update(Time.deltaTime);
                    yield return aszUnityCoroutine.WaitForEndOfFrame;
                }
                current.Finish(Result.Success);

                m_IsRunning = false;
                while (m_IsPaused)
                {
                    yield return aszUnityCoroutine.WaitForEndOfFrame;
                }
                m_IsRunning = true;
            }

            m_IsRunning = false;
        }
    }
}