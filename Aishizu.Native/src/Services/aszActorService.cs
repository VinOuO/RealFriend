using System.Collections.Generic;
using System.Linq;

namespace Aishizu.Native.Services
{
    public readonly struct aszActorData
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Description;
        public readonly string Tags; // Optional keywords for LLM hinting

        [System.Text.Json.Serialization.JsonConstructor]
        public aszActorData(int id, string name, string description = "", string tags = "")
        {
            Id = id;
            Name = name;
            Description = description;
            Tags = tags;
        }

        public override string ToString() => $"{Id}: {Name} ({Tags})";
    }

    public class aszActorService
    {
        private readonly Dictionary<int, aszActorData> m_ActorList = new();

        public void RegisterActor(int id, string name, string description = "", string tags = "")
        {
            m_ActorList[id] = new aszActorData(id, name, description, tags);
        }

        public IReadOnlyDictionary<int, aszActorData> GetAll() => m_ActorList;
        public bool TryGetActor(int id, out aszActorData actor) => m_ActorList.TryGetValue(id, out actor);

        public string ToJson() => new ActorListWrapper(m_ActorList.Values.ToList()).ToJson();

        [System.Serializable]
        private readonly struct ActorListWrapper
        {
            public readonly List<aszActorData> Actors;
            public ActorListWrapper(List<aszActorData> list) => Actors = list;
        }
    }
}
