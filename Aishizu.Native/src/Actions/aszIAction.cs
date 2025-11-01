using System;

namespace Aishizu.Native.Actions
{
    /// <summary>
    /// Defines the full lifecycle of an Aishizu action,
    /// controlled explicitly by the developer.
    /// </summary>
    public enum aszActionState
    {
        /// <summary>
        /// The initial state.  
        /// The action has been created but not yet started or prepared.
        /// </summary>
        Idle,

        /// <summary>
        /// The action cannot run — blocked by external factors,  
        /// such as conflicting body parts or other running actions.
        /// </summary>
        Blocked,

        /// <summary>
        /// The developer is preparing the action,  
        /// such as aligning the character, moving into position, or waiting for conditions.
        /// </summary>
        Preparing,

        /// <summary>
        /// The action is actively running.  
        /// At this stage, the developer has signaled that the action is performing
        /// its intended behavior (e.g., playing an animation or executing logic).
        /// </summary>
        Running,

        /// <summary>
        /// The action is transitioning toward completion,  
        /// performing cleanup or exit behaviors before marking success or failure.
        /// </summary>
        Cleaning,

        /// <summary>
        /// The action finished successfully.  
        /// Developers should mark this once all cleanup and undo logic are complete.
        /// </summary>
        Succeed,

        /// <summary>
        /// The action has failed — either aborted or encountered an unrecoverable issue.  
        /// This state should be explicitly set by the developer if the action cannot complete.
        /// </summary>
        Failed
    }


    public interface aszIAction
    {
        /// <summary>Unique action identifier used by the LLM and SDK.</summary>
        string ActionName { get; set; }

        /// <summary>Target index assigned at runtime by the LLM or manager.</summary>
        int TargetId { get; set; }
    }

    [Serializable]
    public abstract class aszAction : aszIAction
    {
        private int m_ActorId = -1;
        private int m_TargetId = -1;
        private string m_ActionName = "";
        /// <summary>
        /// Flag to allow the user to info native the status of the action
        /// </summary>
        private aszActionState m_State = aszActionState.Idle;

        public int ActorId
        {
            get => m_ActorId;
            set => m_ActorId = value;
        }

        public int TargetId
        {
            get => m_TargetId;
            set => m_TargetId = value;
        }

        public string ActionName
        {
            get => m_ActionName;
            set => m_ActionName = value;
        }

        public aszActionState State
        {
            get => m_State;
            set => m_State = value;
        }

        public bool IsValid { get; protected set; } = false;

        public virtual void Start()
        {
            OnStart();
        }


        public virtual void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public virtual void Finish()
        {
            OnFinish();
        }

        // 🔹 Internal abstract hooks for subclass logic
        protected virtual void OnStart() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnFinish() { }

        public void SetState(aszActionState newState)
        {
            if (State != newState)
            {
                aszLogger.WriteLine($"[aszAction] {GetType().Name} state changed: {State} → {newState}");
                State = newState;
            }
        }


        // 🔹 Helper property
        public bool IsFinished => m_State == aszActionState.Succeed || m_State == aszActionState.Failed;
    }
}