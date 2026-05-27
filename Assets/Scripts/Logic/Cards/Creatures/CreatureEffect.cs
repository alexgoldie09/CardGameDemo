using UnityEngine;

public abstract class CreatureEffect
{
    protected Player owner; // who this creature belongs to
    protected CreatureLogic creature; // the creature this effect is attached to
    protected int specialAmount; // the amount needed to do the effect

    /// <summary>
    /// The constructor for the CreatureEffect class.
    /// We need to pass in the owner of the creature, the creature itself, and the special amount for the effect.
    /// The special amount is used for effects that require a certain amount of something to trigger,
    /// such as "Deal 3 damage to all enemy creatures at the end of your turn" or "Give a friendly creature +2/+2 at the end of your turn".
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="creature"></param>
    /// <param name="specialAmount"></param>
    public CreatureEffect(Player owner, CreatureLogic creature, int specialAmount)
    {
        this.creature = creature;
        this.owner = owner;
        this.specialAmount = specialAmount;
    }

    #region Special FX Events
    /// <summary>
    /// This method is for registering the event listeners for the special effect.
    /// For example, if we have a special effect that triggers when a creature dies, we would register the event listener for that event in this method.
    /// </summary>
    public virtual void RegisterEventEffect() { }

    /// <summary>
    /// This method is for unregistering the event listeners for the special effect.
    /// We need to do this when the creature dies or is removed from the table, otherwise we might have null reference exceptions when the event is triggered and the creature is no longer there.
    /// </summary>
    public virtual void UnRegisterEventEffect() { }

    /// <summary>
    /// This method is for causing the effect of the special effect when the event is triggered.
    /// </summary>
    public virtual void CauseEventEffect() { }

    #endregion

    #region Special FX Triggers
    /// <summary>
    /// This method is for the battlecry effect of the creature.
    /// It is triggered when the creature is played from the hand onto the table.
    /// </summary>
    public virtual void WhenACreatureIsPlayed() { }
    
    /// <summary>
    /// This method is for the deathrattle effect of the creature.
    /// It is triggered when the creature dies and is removed from the table.
    /// </summary>
    public virtual void WhenACreatureDies() { }

    #endregion
}
