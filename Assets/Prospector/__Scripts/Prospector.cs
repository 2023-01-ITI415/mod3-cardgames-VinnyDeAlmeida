using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // We'll need this line later in the chapter

[RequireComponent(typeof(Deck))]

[RequireComponent(typeof(JsonParseLayout))]
public class Prospector : MonoBehaviour
{
    private static Prospector S;

    [Header("Dynamic")]
    public List<CardProspector> drawPile;

    private Deck deck;
    private JsonLayout jsonLayout;

    // Start is called before the first frame update
    void Start()
    {
        if (S != null) Debug.LogError("Attermpe to set S more than once!");
        S = this;

        jsonLayout = GetComponent<JsonParseLayout>().layout;

        deck = GetComponent<Deck>();
        // These two lines replace the Start() call we commented out in Deck
        deck.InitDeck();
        Deck.Shuffle(ref deck.cards);
        drawPile = ConvertCardsToCardProspectors(deck.cards);
    }

    /// <summary>
    /// Converts each Card in a List(Card) into a List(CardProspector) so that it
    /// can be used in the Prospector game.
    /// </summary>
    List<CardProspector>
    ConvertCardsToCardProspectors(List<Card> listCard)
    {
        List<CardProspector> listCP = new List<CardProspector>();
        CardProspector cp;
        foreach(Card card in listCard)
        {
            cp = card as CardProspector;

            listCP.Add(cp);
        }
        return (listCP);
    }
}
