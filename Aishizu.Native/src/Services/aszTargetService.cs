using System.Collections.Generic;
using System.Linq;

namespace Aishizu.Native.Services
{
    /// <summary>
    /// Lightweight data record for scene interactables / targets.
    /// </summary>
    public readonly struct aszTargetData
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Description;
        public readonly string Type; // optional: for categorization (e.g., SitSpot, Hugable)

        [System.Text.Json.Serialization.JsonConstructor]
        public aszTargetData(int id, string name, string description = "", string type = "")
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
        }

        public override string ToString() => $"{Id}: {Name} ({Type})";
    }

    /// <summary>
    /// Native service for managing and serializing available interaction targets.
    /// </summary>
    public class aszTargetService
    {
        private readonly Dictionary<int, aszTargetData> m_TargetList = new();

        /// <summary>
        /// Registers a new interactable target into the service.
        /// </summary>
        public void RegisterTarget(int id, string name, string description = "", string type = "")
        {
            m_TargetList[id] = new aszTargetData(id, name, description, type);
        }

        public void CleanTargetList()
        {
            m_TargetList.Clear();
        }

        /// <summary>
        /// Returns a read-only collection of all registered targets.
        /// </summary>
        public IReadOnlyDictionary<int, aszTargetData> GetAll() => m_TargetList;

        /// <summary>
        /// Attempts to retrieve a specific target by ID.
        /// </summary>
        public bool TryGetTarget(int id, out aszTargetData target) => m_TargetList.TryGetValue(id, out target);

        /// <summary>
        /// Serializes the full list of registered targets into a JSON schema for the LLM.
        /// </summary>
        public string ToJson() => new TargetListWrapper(m_TargetList.Values.ToList()).ToJson();

        [System.Serializable]
        private readonly struct TargetListWrapper
        {
            public readonly List<aszTargetData> Targets;
            public TargetListWrapper(List<aszTargetData> list) => Targets = list;
        }
    }
}
