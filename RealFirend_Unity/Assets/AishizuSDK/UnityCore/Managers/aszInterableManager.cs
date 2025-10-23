using Aishizu.Native;
using System.Collections.Generic;
using UnityEngine;

namespace Aishizu.UnityCore
{
    public class aszInterableManager : MonoBehaviour
    {
        [SerializeField] private List<aszInteractable> m_InterableList; public List<aszInteractable> InterableList => m_InterableList;

        public Result GetInterable(int id, out aszInteractable interable)
        {
            if(m_InterableList.Count > id && id >= 0)
            {
                interable = m_InterableList[id];
                return Result.Success;
            }
            interable = null;
            return Result.Failed;
        }
    }
}
