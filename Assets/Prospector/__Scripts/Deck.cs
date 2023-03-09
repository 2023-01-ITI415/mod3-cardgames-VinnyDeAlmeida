using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JsonParseDeck))]

public class Deck : MonoBehaviour
{
    [Header("Inscribed")]
    public CardSpritesSO cardSprites;
    public GameObject prefabCard;
    public GameObject prefabSprite;
    public bool startFaceUp = true;

    [Header("Dynamic")]
    public Transform deckAnchor;
    public List<Card> cards;
    private JsonParseDeck jsonDeck;
    static public GameObject SPRITE_PREFAB { get; private set; }

    // Start is called before the first frame update
    /* void Start()
    {
        InitDeck();
        Shuffle(ref cards);
    } */

    /// <summary>
    /// The Proespector class will call InitDeck to set up the deck and build
    /// all 52 card GameObjects fom the jsonDeck and cardSprites information.
    /// </summary>
    
    public void InitDeck()
    {
        // Create a static reference to spritePrefab for the Card class to use
        SPRITE_PREFAB = prefabSprite;
        // Call Init method on the CardSpriteSO instance assigned to cardSprites
        cardSprites.Init();

        // Get a reference to the JsonParseDeck component
        jsonDeck = GetComponent<JsonParseDeck>();

        // Create an anchor fo all the Card GameObjects in the Hierarchy
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }

        MakeCards();
    }

    /// <summary>
    /// Create a GameObject for each card in the deck.
    /// </summary>

    void MakeCards()
    {
        cards = new List<Card>();
        Card c;

        // Generate 13 cards for each suit
        string suits = "CDHS";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 13;  j++)
            {
                c = MakeCard(suits[i], j);

                cards.Add(c);

                // This aligns the cards in nice rows for testing
                c.transform.position = new Vector3((j - 7) * 3, (i - 1.5f) * 4, 0);
            }
        }
    }

    /// <summary>
    /// Creates a card GameObject based on suit and rank.
    /// Note that this method assumes it will be passed a valid suit and rank.
    /// </summary>
    /// 
    Card MakeCard(char suit, int rank)
    {
        GameObject go = Instantiate<GameObject>(prefabCard, deckAnchor);
        Card card = go.GetComponent<Card>();

        card.Init(suit, rank, startFaceUp);

        return card;
    }

    /// <summary>
    /// Suffle a List(Card) and return the result to the original list.
    /// </summary>
    static public void Shuffle(ref List<Card> refCards)
    {
        // Create a temporary List to hold the new shuffle order
        List<Card> tCards = new List<Card>();

        int ndx; // This will hold the index of the card to be moved
        // Repeat as long as there are cards in the original List
        while (refCards.Count > 0)
        {
            // Pick the index of a random card
            ndx = Random.Range(0, refCards.Count);
            // Add that card to the temporary List
            tCards.Add(refCards[ndx]);
            // And remove that card from the original List
            refCards.RemoveAt(ndx);
        }
        // Replace the original List with the temporary List
        refCards = tCards;
    }
}
