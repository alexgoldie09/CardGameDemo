using UnityEngine;
using System.Collections;

public class DealDamageCommand : Command 
{

    private int targetID;
    private int amount;
    private int healthAfter;

    /// <summary>
    /// Constructor to deal damage command.
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="amount"></param>
    /// <param name="healthAfter"></param>
    public DealDamageCommand( int targetID, int amount, int healthAfter)
    {
        this.targetID = targetID;
        this.amount = amount;
        this.healthAfter = healthAfter;
    }

    /// <summary>
    /// Starts the execution of the deal damage command.
    /// It retrieves the target game object using the target ID and applies damage to it.
    /// If the target is a hero, it calls the TakeDamage method on the PlayerPortraitVisual component.
    /// If the target is a creature, it calls the TakeDamage method on the CreatureInfoManager component.
    /// Finally, it signals that the command execution is complete.
    /// </summary>
    public override void StartCommandExecution()
    {
        Debug.Log("In deal damage command!");

        GameObject target = IDHolder.GetGameObjectWithID(targetID);
        if (targetID == GlobalSettings.Instance.LowPlayer.ID || 
            targetID == GlobalSettings.Instance.TopPlayer.ID)
        {
            // target is a hero
            target.GetComponent<PlayerPortraitVisual>().TakeDamage(amount,healthAfter);
        }
        else
        {
            // target is a creature
            target.GetComponent<CreatureInfoManager>().TakeDamage(amount, healthAfter);
        }
        CommandExecutionComplete();
    }
}
