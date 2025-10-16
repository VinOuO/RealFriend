namespace Aishizu.Native
{
    [System.Serializable]
    public struct aszVector3
    {
        public float X;
        public float Y;
        public float Z;

        public aszVector3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public static aszVector3 Zero => new aszVector3(0, 0, 0);

        public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";
    }
}

