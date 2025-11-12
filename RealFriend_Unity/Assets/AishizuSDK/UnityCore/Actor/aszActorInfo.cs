using System;

namespace Aishizu.UnityCore
{
    [Serializable]
    public struct aszActorInfo
    {
        public aszActor Actor;
        public string Name;
        public string Discription;

        public aszActorInfo(aszActor actor, string name, string discription)
        {
            Actor = actor;
            Name = name;
            Discription = discription;
        }
    }
}
