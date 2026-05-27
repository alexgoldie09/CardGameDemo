using UnityEngine;
using System.Collections;

public class DamageAllOpponentCreatures : SpellEffect 
{

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null)
    {
        CreatureLogic[] creaturesToDamage = TurnManager.Instance.WhoseTurn.OtherPlayer.Table.CreaturesOnTable.ToArray();
        foreach (var cl in creaturesToDamage)
        {
            new DealDamageCommand(cl.ID, specialAmount, healthAfter: cl.Health - specialAmount).AddToQueue();
            cl.Health -= specialAmount;
        }
    }
}
