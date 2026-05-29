using UnityEngine;

public class BiteOwner : CreatureEffect
{
    public BiteOwner(Player owner, CreatureLogic creature, int specialAmount) 
        : base(owner, creature, specialAmount) {}

    public override void RegisterEventEffect()
    {
        owner.OnEndTurn += CauseEventEffect;
        Debug.Log("Registered bite effect!!!!");
    }

    public override void UnRegisterEventEffect()
    {
        owner.OnEndTurn -= CauseEventEffect;
    }

    public override void CauseEventEffect()
    {
        Debug.Log("InCauseEffect: owner: " + owner.OtherPlayer + " specialAmount: " + specialAmount);
        new DealDamageCommand(owner.OtherPlayer.ID, specialAmount, owner.OtherPlayer.Health - specialAmount).AddToQueue();
        owner.OtherPlayer.Health -= specialAmount;
    }
}