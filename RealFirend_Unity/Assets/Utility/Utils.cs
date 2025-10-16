using UnityEngine;


public struct MouthShape
{
    public char Vowel1, Vowel2;
    public float Weight1, Weight2;
    public static MouthShape Default = new MouthShape(' ', ' ', 0, 0);
    public MouthShape(char vowel1, char vowel2, float weight1, float weight2)
    {
        Vowel1 = vowel1;
        Vowel2 = vowel2;
        Weight1 = weight1;
        Weight2 = weight2;
    }
}

public struct FacialBlend
{
    public FacialExpression Expression1, Expression2;
    public float Weight1, Weight2;
    public static FacialBlend Default = new FacialBlend(FacialExpression.Natural, FacialExpression.Natural, 0, 0);
    public FacialBlend(FacialExpression expression1, FacialExpression expression2, float weight1, float weight2)
    {
        Expression1 = expression1;
        Expression2 = expression2;
        Weight1 = weight1;
        Weight2 = weight2;
    }
}