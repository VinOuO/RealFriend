using System;

namespace Aishizu.UnityCore
{
    [Serializable]
    public struct aszInteractableInfo
    {
        public aszInteractable Interable;
        public string Name;
        public string Discription;

        public aszInteractableInfo(aszInteractable interactable, string name, string discription)
        {
            Interable = interactable;
            Name = name;
            Discription = discription;
        }
    }
}
