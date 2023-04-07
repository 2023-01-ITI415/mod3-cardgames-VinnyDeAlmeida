using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSprites", menuName = "ScriptableObjects/CardSpritesSO")]
public class CardSpritesSO : ScriptableObject
{
    [Header("Card Stock")]
    public Sprite cardBack;
    public Sprite cardBackSilver;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontSilver;
    public Sprite cardFrontGold;

    [Header("Suits")]
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    [Header("Pip Sprites")]
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    private static CardSpritesSO S;
    public static Dictionary<char, Sprite> SUITS
    {
        get; private set;
    }

    public void Init()
    {
        INIT_STATICS(this);
    }

    /// <summary>
    /// Initializes the static elements of CardSpritesSO.
    /// </summary>

    static void INIT_STATICS(CardSpritesSO cSS0)
    {
        if (S != null)
        {
            Debug.LogError("CardSpritesSO.S can't be set a 2nd time!");
            return;
        }
        S = cSS0;

        // Initialize the _SUITS Dictionary
        SUITS = new Dictionary<char, Sprite>()
        {
            { 'C', S.suitClub },
            { 'D', S.suitDiamond },
            { 'H', S.suitHeart },
            { 'S', S.suitSpade },
        };
    }

    public static Sprite[] RANKS
    {
        get { return S.rankSprites; }
    }

    /// <summary>
    /// Searches S.faceSprites for the one with the right name
    /// </summary>
    
    public static Sprite GET_FACE(string name)
    {
        foreach (Sprite spr in S.faceSprites)
        {
            if (spr.name == name) return spr;
        }
        return null;
    }

    /// <summary>
    /// This public static property makes S.cardBack accessible to other classes
    /// </summary>
    
    public static Sprite BACK
    {
        get { return S.cardBack; }
    }
    
    public static Sprite GET_SILVER_BACK
    {
        get { return S.cardBackSilver; }
    }

    public static Sprite GET_SILVER_FRONT
    {
        get { return S.cardFrontSilver; }
    }

    public static Sprite GET_GOLD_BACK
    {
        get { return S.cardBackGold; }
    }

    public static Sprite GET_GOLD_FRONT
    {
        get { return S.cardFrontGold; }
    }

    public static void RESET()
    {
        S = null;
    }
}
