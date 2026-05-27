using UnityEngine;
using System.Collections;

public class HeroPowerDrawCardTakeDamage : SpellEffect 
{

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null)
    {
        // Take 2 damage
        new DealDamageCommand(TurnManager.Instance.WhoseTurn.ID, 2, TurnManager.Instance.WhoseTurn.Health - 2).AddToQueue();
        TurnManager.Instance.WhoseTurn.Health -= 2;
        // Draw a card
        TurnManager.Instance.WhoseTurn.DrawACard();

    }
}
