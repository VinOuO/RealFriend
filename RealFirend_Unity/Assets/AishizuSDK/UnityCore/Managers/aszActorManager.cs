using System.Collections.Generic;
using UnityEngine;
using Aishizu.Native;

namespace Aishizu.UnityCore
{
    public class aszActorManager : MonoBehaviour
    {
        [SerializeField] private List<aszActor> m_ActorList; public List<aszActor> ActorList => m_ActorList;
        public Result GetActor(int id, out aszActor interable)
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
