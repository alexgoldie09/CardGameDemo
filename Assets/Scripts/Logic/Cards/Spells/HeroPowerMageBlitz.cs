public class HeroPowerMageBlitz : SpellEffect
{
    private const int DamageAmount = 3;

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null)
    {
        CreatureLogic[] creaturesToDamage = TurnManager.Instance.WhoseTurn.OtherPlayer.Table.CreaturesOnTable.ToArray();
        foreach (var cl in creaturesToDamage)
        {
            new DealDamageCommand(cl.ID, DamageAmount, healthAfter: cl.Health - DamageAmount).AddToQueue();
            cl.Health -= DamageAmount;
        }
    }
}