using System.Collections;
using UnityEngine;
using Aishizu.Native.Actions;

namespace Aishizu.UnityCore
{

    public class aszCharacterController : MonoBehaviour
    {
        public virtual IEnumerator Execute(aszIUnityAction action)
        {
            Debug.Log($"[CharacterController] Executing {action.Name}");
            yield return null;
        }
    }
}
