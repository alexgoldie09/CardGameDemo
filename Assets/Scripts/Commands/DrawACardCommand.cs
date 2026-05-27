using UnityEngine;
using System.Collections;

public class DrawACardCommand : Command 
{

    private Player p;
    private CardLogic cl;
    private bool fast;
    private bool fromDeck;

    /// <summary>
    /// Constructor for DrawACardCommand. 
    /// </summary>
    /// <param name="cl"></param>
    /// <param name="p"></param>
    /// <param name="fast"></param>
    /// <param name="fromDeck"></param>
    public DrawACardCommand(CardLogic cl, Player p, bool fast, bool fromDeck)
    {        
        this.cl = cl;
        this.p = p;
        this.fast = fast;
        this.fromDeck = fromDeck;
    }

    /// <summary>
    /// Starts the execution of the command. Decreases the number of cards in the deck and gives the player a card in their hand.
    /// </summary>
    public override void StartCommandExecution()
    {
        p.PArea.PDeck.CardsInDeck--;
        p.PArea.HandVisual.GivePlayerACard(cl.CA, cl.ID, fast, fromDeck);
    }
}
