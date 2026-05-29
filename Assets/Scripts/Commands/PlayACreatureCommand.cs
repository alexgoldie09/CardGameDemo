using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayACreatureCommand : Command
{
    private CardLogic cl;
    private int tablePos;
    private Player p;
    private int creatureID;

    /// <summary>
    /// Constructor for playing a creature card.
    /// </summary>
    /// <param name="cl"></param>
    /// <param name="p"></param>
    /// <param name="tablePos"></param>
    /// <param name="creatureID"></param>
    public PlayACreatureCommand(CardLogic cl, Player p, int tablePos, int creatureID)
    {
        this.p = p;
        this.cl = cl;
        this.tablePos = tablePos;
        this.creatureID = creatureID;
    }

    /// <summary>
    /// Executes the command to play a creature card.
    /// It removes the card from the player's hand, destroys the card GameObject,
    /// enables hover previews, and adds the creature to the table at the specified position.
    /// </summary>
    public override void StartCommandExecution()
    {
        // remove and destroy the card in hand 
        HandVisual playerHand = p.PArea.HandVisual;
        GameObject card = IDHolder.GetGameObjectWithID(cl.ID);
        playerHand.RemoveCard(card);
        // foreach (Transform t in card.GetComponentsInChildren<Transform>())
        //     DOTween.Kill(t);
        GameObject.Destroy(card);
        // enable Hover Previews Back
        HoverPreview.PreviewsAllowed = true;
        // move this card to the spot 
        p.PArea.TableVisual.AddCreatureAtIndex(cl.CA, creatureID, tablePos);
    }
}
