using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Dynamic")]
    public char suit; // Suit of the card (C, D, H, or S)
    public int rank; // Rank of the card (1-13)
    public Color color = Color.black; // Color to tint pips
    public string cols = "Black"; // or "red". Name of the Color
    public GameObject back; // The GameObject of the back of the card
    public JsonCard def; // The card layout as defined in JSON_Deck.json

    // This List holds all of the Decorator GameObjects
    public List<GameObject> decoGOs = new List<GameObject>();
    // This List holds all of the Pip GameObjects
    public List<GameObject> pipGOs = new List<GameObject>();

    /// <summary>
    /// Creates this Card's visuals based on suit and rank.
    /// Note that this method assumes it will be passed a valid suit and rank.
    /// </summary>
    public void Init(char eSuit, int eRank, bool startFaceUp = true)
    {
        // Assign basic alues to the Card
        gameObject.name = name = eSuit.ToString() + eRank;
        suit = eSuit;
        rank = eRank;
        // If this is a Diamond or Heart, change the default Black color to red
        if (suit == 'D' || suit == 'H')
        {
            cols = "Red";
            color = Color.red;
        }

        def = JsonParseDeck.GET_CARD_DEF(rank);

        // Build the card from Sprites
    }

    /// <summary>
    /// Shortcut for setting transform.localPosition.
    /// </summary>
    
    public virtual void SetLocalPos(Vector3 v)
    {
        transform.localPosition = v;
    }
}
