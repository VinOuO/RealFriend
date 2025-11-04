namespace Aishizu.Native.Events
{
    public class aszEmotionChange : aszIEvent
    {
        public int ActorId { get; set; }
        public aszEmotion Emotion { get; set; }
        public float Duration { get; set; } = 2.0f; // Optional fade time
    }
}

