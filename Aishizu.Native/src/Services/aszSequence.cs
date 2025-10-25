using Aishizu.Native.Actions;
using System.Collections.Generic;

namespace Aishizu.Native
{
    public class aszSequence
    {
        private readonly Queue<aszAction> m_ActionQueue = new();
        private aszAction? m_Current;
        private bool m_IsRunning; public bool IsRunning => m_IsRunning;
        private bool m_IsPaused; public bool IsPaused => m_IsPaused;

        public aszSequence(List<aszAction> actions) 
        {
            for (int i = 0; i < actions.Count; i++)
            {
                Enqueue(actions[i]);
            }
            m_IsRunning = false;
            m_IsPaused = false;
        }

        public void Enqueue(aszAction action)
        {
            m_ActionQueue.Enqueue(action);
        }

        public void Start()
        {
            if (m_IsRunning)
            {
                return;
            }
            m_IsRunning = true;
            NextAction();
        }

        public void Kill()
        {
            m_IsRunning = false;
            m_ActionQueue.Clear();
            m_Current = null;
            m_IsPaused = false;
        }

        public void Pause(bool pause)
        {
            if (m_IsRunning)
            {
                m_IsPaused = pause;
            }
            else
            {
                m_IsPaused = false;
            }
        }

        public void Tick(float deltaTime)
        {
            if (!m_IsRunning || m_IsPaused || m_Current == null)
            {
                return;
            }

            if (!m_Current.IsFinished)
            {
                m_Current.Update(deltaTime);
            }
            else
            {
                m_Current.Finish(Result.Success);
                NextAction();
            }
        }

        private void NextAction()
        {
            if (m_ActionQueue.Count > 0)
            {
                m_Current = m_ActionQueue.Dequeue();
                m_Current.Start();
            }
            else
            {
                m_IsRunning = false;
                m_Current = null;
            }
        }
    }
}
