namespace Aishizu.Native.Events
{
    public class aszEmotionChange : aszEvent
    {
        public int ActorId { get; set; }
        public aszEmotion Emotion { get; set; }
        public float Duration { get; set; } = 2.0f; // Optional fade time

        public aszEmotionChange() 
        {
            Name = "EmotionChange";
        }
    }
}

