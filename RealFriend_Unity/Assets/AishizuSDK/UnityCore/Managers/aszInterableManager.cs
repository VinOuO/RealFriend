using Aishizu.Native;
using System.Collections.Generic;
using UnityEngine;

namespace Aishizu.UnityCore
{
    public class aszInterableManager : MonoBehaviour
    {
        [SerializeField] private List<aszInteractableInfo> m_InterableList; public List<aszInteractableInfo> InterableList => m_InterableList;

        public Result GetInterable(int id, out aszInteractable interable)
        {
            if(m_InterableList.Count > id && id >= 0)
            {
                interable = m_InterableList[id].Interable;
                return Result.Success;
            }
            interable = null;
            return Result.Failed;
        }

        public Result AddInterable(aszInteractableInfo interable)
        {
            m_InterableList.Add(interable);
            return Result.Success;
        }
    }
}
