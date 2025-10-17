
namespace Aishizu.UnityCore
{
    public enum FacialExpression
    {
        Null = 0,

        //---Expression
        Natural = 1 << 0,
        Angry = 1 << 1,
        Happy = 1 << 2,
        Sad = 1 << 3,
        Surprised = 1 << 4,

        //---Vowel
        NoVowel = 1 << 10,
        A = 1 << 11,
        E = 1 << 12,
        I = 1 << 13,
        O = 1 << 14,
        U = 1 << 15,
    }
}
