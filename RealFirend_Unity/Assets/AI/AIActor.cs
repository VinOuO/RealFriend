using UnityEngine;
using static AIMediator;

public class AIActor : MonoBehaviour
{
    public static AIActor Instance;

    [Header("Debug")]
    [SerializeField]
    ActorBehavior CurrentBehavior;

    void OnEnable()
    {
        Instance = this;
    }

    void Start()
    {

    }

    [ContextMenu("GetBehavior")]
    public void GetBehavior()
    {
        if (AIMediator.Instance.CurrentResponse != null)
        {
            CurrentBehavior = AIMediator.Instance.CurrentResponse.choices[0].message.content.ToActorBehavior();
        }
    }

    [System.Serializable]
    public class ActorBehavior
    {
        public bool IsValid = true;
        public string Response;
        public BehaviorAction Action;
        public BehaviorTarget Target;
    }

    public enum BehaviorAction
    {
        hug, kiss, touch, look, idle
    }

    public enum BehaviorTarget
    {
        nothing, player_lips, player_cheek, player_hand, player_waist, player_face,
    }

}
