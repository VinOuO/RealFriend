using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aishizu.Native.Actions;
using Aishizu.Native.Sequencer;

namespace Aishizu.UnityCore.Sequencer
{
    /// <summary>
    /// Simple sequential executor that runs Aishizu Native actions one after another.
    /// </summary>
    public class aszUnitySequencer : MonoBehaviour, ISequencer
    {
        private readonly Queue<IAction> m_Queue = new();
        private bool m_IsRunning;
        private bool m_IsPaused;

        public void Enqueue(IAction action)
        {
            m_Queue.Enqueue(action);
        }

        public void Enqueue(IEnumerable<IAction> actions)
        {
            foreach (IAction action in actions)
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
                IAction current = m_Queue.Dequeue();
                yield return Execute(current);
                m_IsRunning = false;
                while (m_IsPaused)
                {
                    yield return aszUnityCoroutine.WaitForEndOfFrame;
                }
                m_IsRunning = true;
            }

            m_IsRunning = false;
        }

        private IEnumerator Execute(IAction action)
        {
            switch (action)
            {
                case ActionWalk walk:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing WalkAction ¡÷ Target: {walk.TargetId}, StopDistance: {walk.StopDistance}");
                    yield return aszUnityCoroutine.WaitForSeconds(2.0f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished WalkAction.");
                    break;

                case ActionReach reach:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing ReachAction ¡÷ Target: {reach.TargetId}, Hand: {reach.Hand}");
                    yield return aszUnityCoroutine.WaitForSeconds(1.5f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished ReachAction.");
                    break;

                case ActionTouch touch:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing TouchAction ¡÷ Target: {touch.TargetId}, Hand: {touch.Hand}");
                    yield return aszUnityCoroutine.WaitForSeconds(1.0f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished TouchAction.");
                    break;

                case ActionHold hold:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing HoldAction ¡÷ Target: {hold.TargetId}");
                    yield return aszUnityCoroutine.WaitForSeconds(1.0f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished HoldAction.");
                    break;

                case ActionSit sit:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing SitAction ¡÷ Target: {sit.TargetId}");
                    yield return aszUnityCoroutine.WaitForSeconds(1.5f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished SitAction.");
                    break;

                case ActionHug hug:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing HugAction ¡÷ Target: {hug.TargetId}");
                    yield return aszUnityCoroutine.WaitForSeconds(1.2f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished HugAction.");
                    break;
                case ActionKiss kiss:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing KissAction ¡÷ Target: {kiss.TargetId}");
                    yield return aszUnityCoroutine.WaitForSeconds(1.2f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished KissAction.");
                    break;

            }
        }
    }
}