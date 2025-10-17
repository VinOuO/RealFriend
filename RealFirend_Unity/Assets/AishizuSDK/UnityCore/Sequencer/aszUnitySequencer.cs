using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aishizu.Native.Actions;
using Aishizu.Native.Sequencer;
using Aishizu.UnityCore.Actions;

namespace Aishizu.UnityCore.Sequencer
{
    /// <summary>
    /// Simple sequential executor that runs Aishizu Native actions one after another.
    /// </summary>
    public class aszUnitySequencer : MonoBehaviour, aszISequencer
    {
        [SerializeField] private aszCharacterController m_Controller;
        private readonly Queue<aszIAction> m_Queue = new();
        private bool m_IsRunning;
        private bool m_IsPaused;

        public void Enqueue(aszIAction action)
        {
            m_Queue.Enqueue(action);
        }

        public void Enqueue(IEnumerable<aszIAction> actions)
        {
            foreach (aszIAction action in actions)
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
                aszIAction current = m_Queue.Dequeue();
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

        private IEnumerator Execute(aszIAction action)
        {
            switch (action)
            {
                case aszActionWalk walk:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing WalkAction ¡÷ Target: {walk.TargetId}, StopDistance: {walk.StopDistance}");
                    m_Controller.Execute(new aszUnityActionWalk(walk));
                    yield return aszUnityCoroutine.WaitForSeconds(2.0f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished WalkAction.");
                    break;

                case aszActionReach reach:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing ReachAction ¡÷ Target: {reach.TargetId}, Hand: {reach.Hand}");
                    m_Controller.Execute(new aszUnityActionReach(reach));
                    yield return aszUnityCoroutine.WaitForSeconds(1.5f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished ReachAction.");
                    break;

                case aszActionTouch touch:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing TouchAction ¡÷ Target: {touch.TargetId}, Hand: {touch.Hand}");
                    m_Controller.Execute(new aszUnityActionTouch(touch));
                    yield return aszUnityCoroutine.WaitForSeconds(1.0f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished TouchAction.");
                    break;

                case aszActionHold hold:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing HoldAction ¡÷ Target: {hold.TargetId}");
                    m_Controller.Execute(new aszUnityActionHold(hold));
                    yield return aszUnityCoroutine.WaitForSeconds(1.0f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished HoldAction.");
                    break;

                case aszActionSit sit:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing SitAction ¡÷ Target: {sit.TargetId}");
                    m_Controller.Execute(new aszUnityActionSit(sit));
                    yield return aszUnityCoroutine.WaitForSeconds(1.5f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished SitAction.");
                    break;

                case aszActionHug hug:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing HugAction ¡÷ Target: {hug.TargetId}");
                    m_Controller.Execute(new aszUnityActionHug(hug));
                    yield return aszUnityCoroutine.WaitForSeconds(1.2f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished HugAction.");
                    break;
                case aszActionKiss kiss:
                    Debug.Log($"<color=#7FFF00>[Sequencer]</color> Executing KissAction ¡÷ Target: {kiss.TargetId}");
                    m_Controller.Execute(new aszUnityActionKiss(kiss));
                    yield return aszUnityCoroutine.WaitForSeconds(1.2f);
                    Debug.Log($"<color=#00FFFF>[Sequencer]</color> Finished KissAction.");
                    break;

            }
        }
    }
}