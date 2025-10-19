using System.Collections.Generic;
using UnityEngine;
using Aishizu.Native;

namespace Aishizu.UnityCore
{
    public class aszActorManager : MonoBehaviour
    {
        public static aszActorManager Instance;
        [SerializeField] private List<aszCharacter> m_ActorList; public List<aszCharacter> ActorList => m_ActorList;

        private void OnEnable()
        {
            Instance = this;
        }

        public Result GetActor(int id, out aszCharacter interable)
        {
            if (m_ActorList.Count > id && id >= 0)
            {
                interable = m_ActorList[id];
                return Result.Success;
            }
            interable = null;
            return Result.Failed;
        }
    }
}
