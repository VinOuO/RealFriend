using System;

namespace Aishizu.Native.Actions
{
    [Serializable]
    public abstract class aszAction
    {
        private int m_ActorId = -1;
        private int m_TargetId = -1;
        private string m_ActionName = "";

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
    }
}