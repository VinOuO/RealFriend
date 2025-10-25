using System;

namespace Aishizu.Native.Actions
{
    public enum aszActionState
    {
        Idle,
        Running,
        Success,
        Failed,
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

        public bool IsValid { get; protected set; } = false;
        public aszActionState State => m_State;

        // 🔹 Called once when the action begins
        public virtual void Start()
        {
            m_State = aszActionState.Running;
            OnStart();
        }

        // 🔹 Called each update tick by the Sequencer
        public virtual void Update(float deltaTime)
        {
            if (m_State != aszActionState.Running) return;

            OnUpdate(deltaTime);
        }

        // 🔹 Called when the action is forced to stop (optional)
        public virtual void Finish(Result finishStatus)
        {
            OnFinish(finishStatus);
            m_State = aszActionState.Failed;
        }

        public void SetFinish(Result finishStatus)
        {
            m_State = finishStatus == Result.Success ? aszActionState.Success : aszActionState.Failed;
        }

        // 🔹 Internal abstract hooks for subclass logic
        protected virtual void OnStart() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnFinish(Result finishStatus) { }

        // 🔹 Helper property
        public bool IsFinished => m_State == aszActionState.Success || m_State == aszActionState.Failed;
    }
}