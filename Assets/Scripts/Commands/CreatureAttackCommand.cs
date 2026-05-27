using UnityEngine;
using System.Collections;

public class CreatureAttackCommand : Command 
{
    // position of creature on enemy`s table that will be attacked
    // if enemyindex == -1 , attack an enemy character 
    private int TargetUniqueID;
    private int AttackerUniqueID;
    private int AttackerHealthAfter;
    private int TargetHealthAfter;
    private int DamageTakenByAttacker;
    private int DamageTakenByTarget;

    /// <summary>
    /// Constructor for a command that initiates a creature attack.
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="attackerID"></param>
    /// <param name="damageTakenByAttacker"></param>
    /// <param name="damageTakenByTarget"></param>
    /// <param name="attackerHealthAfter"></param>
    /// <param name="targetHealthAfter"></param>
    public CreatureAttackCommand(int targetID, int attackerID, int damageTakenByAttacker, int damageTakenByTarget, int attackerHealthAfter, int targetHealthAfter)
    {
        this.TargetUniqueID = targetID;
        this.AttackerUniqueID = attackerID;
        this.AttackerHealthAfter = attackerHealthAfter;
        this.TargetHealthAfter = targetHealthAfter;
        this.DamageTakenByTarget = damageTakenByTarget;
        this.DamageTakenByAttacker = damageTakenByAttacker;
    }

    /// <summary>
    /// Animates the attack of the creature to the target and updates health text of
    /// both attacker and target and creates damage effect if needed.
    /// </summary>
    public override void StartCommandExecution()
    {
        GameObject Attacker = IDHolder.GetGameObjectWithID(AttackerUniqueID);

        //Debug.Log(TargetUniqueID);
        Attacker.GetComponent<CreatureAttackVisual>().AttackTarget(TargetUniqueID, DamageTakenByTarget, DamageTakenByAttacker, AttackerHealthAfter, TargetHealthAfter);
    }
}
