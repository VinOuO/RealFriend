using System.Collections.Generic;
using UnityEngine;
using Aishizu.Native;
using System;

namespace Aishizu.UnityCore
{
    public class aszActorManager : MonoBehaviour
    {
        [SerializeField] private List<aszActor> m_ActorList; public List<aszActor> ActorList => m_ActorList;
        public Result GetActor(int id, out aszActor actor)
        {
            if (m_ActorList.Count > id && id >= 0)
            {
                actor = m_ActorList[id];
                return Result.Success;
            }
            actor = null;
            return Result.Failed;
        }
    }
}
