using UnityEngine;
using System.Collections;

public class PlayerTurnMaker : TurnMaker 
{
    /// <summary>
    /// This method is called at the start of the player`s turn.
    /// It will display a message that it is player`s turn and draw a card for the player.
    /// </summary>
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        // dispay a message that it is player`s turn
        new ShowMessageCommand("Your Turn!", 2.0f).AddToQueue();
        p.DrawACard();
    }
}