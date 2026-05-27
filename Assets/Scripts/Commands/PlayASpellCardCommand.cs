using UnityEngine;
using System.Collections;

public class PlayASpellCardCommand: Command
{
    private CardLogic card;
    private Player p;
    //private ICharacter target;

    /// <summary>
    /// Constructor for the PlayASpellCardCommand, which initializes the command with the player and the card to be played.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="card"></param>
    public PlayASpellCardCommand(Player p, CardLogic card)
    {
        this.card = card;
        this.p = p;
    }

    /// <summary>
    /// Starts the execution of the command, which involves moving the card to the appropriate spot
    /// and performing all necessary visual updates for playing a spell card.
    /// This may include animations, effects, and updating the game state to reflect the card being played.
    /// </summary>
    public override void StartCommandExecution()
    {
        // move this card to the spot
        p.PArea.HandVisual.PlayASpellFromHand(card.ID);
        // do all the visual stuff (for each spell separately????)
    }
}
