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
    public List<CardProspector> discardPile;
    public List<CardProspector> mine;
    public CardProspector target;

    private Transform layoutAnchor;
    private Deck deck;
    private JsonLayout jsonLayout;

    // A Dictionary to pair mine layout IDs and actual Cards
    private Dictionary<int, CardProspector> mineIdToCardDict;

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

        LayoutMine(); // Please don't forget this line

        // Set up the initial target card
        MoveToTarget(Draw());
        // Set up the draw pile
        UpdateDrawPile();
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
        return(listCP);
    }

    CardProspector Draw()
    {
        CardProspector cp = drawPile[0]; // Pull the 0th Card Prospector
        drawPile.RemoveAt(0); // Then remove it from drawPile
        return(cp); // And return it
    }

    void LayoutMine()
    {
        // Create an empty GameObject to serve as an anchor for the tableau
        if(layoutAnchor == null)
        {
            //Create an empty GameObject named _LayoutAnchor in the Hierarchy
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform; // Grab its Transform
        }

        CardProspector cp;

        // Generate the Dictionary to match mine layout ID to CardProspector
        mineIdToCardDict = new Dictionary<int, CardProspector>();

        // Iterate through the JsonLayoutsSlots pulle form the JSON_Layout
        foreach(JsonLayoutSlot slot in jsonLayout.slots)
        {
            cp = Draw(); // Pull a card from the top of the draw Pile
            cp.faceUp = slot.faceUp; // Set its faceUp to the value in SlotDef
            // Make the CardProspector a child of layoutAnchor
            cp.transform.SetParent(layoutAnchor);

            //Convert the last char of the layer string to an int (e.g. "Row 0")
            int z = int.Parse(slot.layer[slot.layer.Length - 1].ToString());

            // Set the localPosition of the card base on the slot information
            cp.SetLocalPos(new Vector3(jsonLayout.multiplier.x * slot.x, jsonLayout.multiplier.y * slot.y, -z));

            cp.layoutID = slot.id;
            cp.layoutSlot = slot;
            cp.state = eCardState.mine;

            cp.SetSpriteSortingLayer(slot.layer);
            mine.Add(cp); // Add this CardProspector to the List<> mine

            // Add this CardProspector to the mineIdtoCardDict Dictionary
            mineIdToCardDict.Add(slot.id, cp);
        }
    }

    void MoveToDiscard(CardProspector cp)
    {
        // Set the state of the card to discard
        cp.state = eCardState.discard;
        discardPile.Add(cp); // Add it to the discardPile List<>
        cp.transform.SetParent(layoutAnchor); // Update its transform parent

        // Position it on the discardPile
        cp.SetLocalPos(new Vector3(jsonLayout.multiplier.x * jsonLayout.discardPile.x, jsonLayout.multiplier.y * jsonLayout.discardPile.y, 0));

        cp.faceUp = true;

        // Place it on top of the pile for dpeth sorting
        cp.SetSpriteSortingLayer(jsonLayout.discardPile.layer);
        cp.SetSortingOrder(-200 + (discardPile.Count * 3));
    }

    /// <summary>
    /// Make cp the new target card
    /// </summary>
    
    void MoveToTarget(CardProspector cp)
    {
        // If there is currently a target card, move it to discardPile
        if (target != null) MoveToDiscard(target);

        // Use MoveToDiscard to move the target card to the correct location
        MoveToDiscard(cp);

        // Then set a few additional things to make cp the new target
        target = cp; // cp is the new target
        cp.state = eCardState.target;

        // Set the depth sorting so that cp is on top of the discardPile
        cp.SetSpriteSortingLayer("Target");
        cp.SetSortingOrder(0);
    }

    /// <summary>
    /// Arranges all the cards of the drawPile to show how many are left
    /// </summary>
    void UpdateDrawPile()
    {
        CardProspector cp;
        // Go through all the cards of the drawPile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cp = drawPile[i];
            cp.transform.SetParent(layoutAnchor);

            // Position it correctly with the layout.drawPile.stagger
            Vector3 cpPos = new Vector3();
            cpPos.x = jsonLayout.multiplier.x * jsonLayout.drawPile.x;
            // Add the staggering for the drawPile
            cpPos.x += jsonLayout.drawPile.xStagger * i;
            cpPos.y = jsonLayout.multiplier.y * jsonLayout.drawPile.y;
            cpPos.z = 0.1f * i;
            cp.SetLocalPos(cpPos);

            cp.faceUp = false; // DrawPile Cards are all face-down
            cp.state = eCardState.drawpile;
            // Set depth sorting

            cp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            cp.SetSortingOrder(-10 * i);
        }
    }

    /// <summary>
    /// This turns cards in the Mine face-up and face-down
    /// </summary>
    public void SetMineFaceUps()
    {
        CardProspector coverCP;
            foreach (CardProspector cp in mine)
        {
            bool faceUp = true; // Assume the card will be face-up

            // Iterate through the covering cards by mine layout ID
            foreach (int coverID in cp.layoutSlot.hiddenBy)
            {
                coverCP = mineIdToCardDict[coverID];
                // If the covering card is null or still in the mine...
                if (coverCP == null || coverCP.state == eCardState.mine)
                {
                    faceUp = false; // then this card is face-down
                }
            }
            cp.faceUp = faceUp; // Set the value on the card
        }
    }

    /// <summary>
    /// Handler for any time a card in the game is clicked
    /// </summary>
    void CheckForGameOver()
    {
        // If the mine is empty, the game is over
        if (mine.Count == 0)
        {
            GameOver(true); // Call GameOver() with a win
            return;
        }

        // If there are still cards in the mine & draw pile, the game's not over
        if (drawPile.Count > 0) return;

        // Check for remaining valid plays
        foreach (CardProspector cp in mine)
        {
            // If there is a valid play, the game's not over
            if (target.AdjacentTo(cp)) return;
        }

        // Since there are no valid plays, the game is over
        GameOver(false); // Call GameOver with a loss
    }

    /// <summary>
    /// Called when the game is over. Simple for now, but expandable
    /// </summary>
    void GameOver(bool won)
    {
        if (won)
        {
            ScoreManager.TALLY(eScoreEvent.gameWin);
        } else
        {
            ScoreManager.TALLY(eScoreEvent.gameLoss);
        }

        CardSpritesSO.RESET();
        // Reload the scene, resetting the game
        // Note that there are TWO underscores at the beginning of "__Prospector...
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

    static public void CARD_CLICKED(CardProspector cp)
    {
        // The reaction is determined by the state of the clicked card
        switch (cp.state)
        {
            case eCardState.target:
                // Clicking the target card does nothing
                break;
            case eCardState.drawpile:
                // Clicking *any* card in the drawPile will draw the next card
                S.MoveToTarget(S.Draw()); // Draw a new target card
                S.UpdateDrawPile(); // Restack the drawPile
                ScoreManager.TALLY(eScoreEvent.draw);
                break;
            case eCardState.mine:
                // Clicking a card in the mine will check if it's a valid play
                bool validMatch = true; // Initially assume that it's valid

                // If the card is face-down, it's not valid
                if (!cp.AdjacentTo(S.target)) validMatch = false;

                if (validMatch)
                {
                    // If it's a valid card
                    S.mine.Remove(cp); // Remove it from the tableau List
                    S.MoveToTarget(cp); // Make it the target card
                    S.SetMineFaceUps(); // Be sure to add this line!
                    ScoreManager.TALLY(eScoreEvent.mine);
                }
                break;
        }

        S.CheckForGameOver(); // This is now the last line of CARD_CLICKED()
    }
}
