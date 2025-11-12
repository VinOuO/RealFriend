using System.Collections.Generic;
using UnityEngine;
using Aishizu.Native;
using System;

namespace Aishizu.UnityCore
{
    public class aszActorManager : MonoBehaviour
    {
        [SerializeField] private List<aszActorInfo> m_ActorList; public List<aszActorInfo> ActorList => m_ActorList;
        public Result GetActor(int id, out aszActor actor)
        {
            if (m_ActorList.Count > id && id >= 0)
            {
                actor = m_ActorList[id].Actor;
                return Result.Success;
            }
            actor = null;
            return Result.Failed;
        }
    }
}
