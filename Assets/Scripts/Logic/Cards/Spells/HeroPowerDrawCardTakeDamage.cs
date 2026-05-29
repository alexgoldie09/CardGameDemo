using UnityEngine;
using System.Collections;

public class HeroPowerDrawCardTakeDamage : SpellEffect 
{

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null)
    {
        // Take 1 damage
        new DealDamageCommand(TurnManager.Instance.WhoseTurn.ID, 1, TurnManager.Instance.WhoseTurn.Health - 1).AddToQueue();
        TurnManager.Instance.WhoseTurn.Health -= 1;
        // Draw a card
        TurnManager.Instance.WhoseTurn.DrawACard();

    }
}
