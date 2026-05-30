using UnityEngine;
using System.Collections;

public class CreatureAttackCommand : Command 
{
    private int TargetUniqueID;
    private int AttackerUniqueID;
    private int AttackerHealthAfter;
    private int TargetHealthAfter;
    private int DamageTakenByAttacker;
    private int DamageTakenByTarget;
    private System.Action _applyDamage;

    /// <summary>
    /// Constructor for creature attack
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="attackerID"></param>
    /// <param name="damageTakenByAttacker"></param>
    /// <param name="damageTakenByTarget"></param>
    /// <param name="attackerHealthAfter"></param>
    /// <param name="targetHealthAfter"></param>
    /// <param name="applyDamage"></param>
    public CreatureAttackCommand(int targetID, int attackerID, int damageTakenByAttacker, 
        int damageTakenByTarget, int attackerHealthAfter, int targetHealthAfter, 
        System.Action applyDamage = null)
    {
        this.TargetUniqueID = targetID;
        this.AttackerUniqueID = attackerID;
        this.AttackerHealthAfter = attackerHealthAfter;
        this.TargetHealthAfter = targetHealthAfter;
        this.DamageTakenByTarget = damageTakenByTarget;
        this.DamageTakenByAttacker = damageTakenByAttacker;
        this._applyDamage = applyDamage;
    }

    /// <summary>
    /// Starts the creature attack command execution. This is where the damage is applied and the attack animation is triggered.
    /// Damage application happens here in StartCommandExecution, so that CreatureDieCommand always gets enqueued after this command is already running.
    /// This ensures that the attack animation plays even if the attack kills the attacker or the target.
    /// </summary>
    public override void StartCommandExecution()
    {
        Debug.Log($"[CreatureAttackCommand] StartCommandExecution called. AttackerID:{AttackerUniqueID} TargetID:{TargetUniqueID}");
        
        GameObject attacker = IDHolder.GetGameObjectWithID(AttackerUniqueID);
        
        if (attacker == null)
        {
            Debug.LogError($"[CreatureAttackCommand] Attacker ID:{AttackerUniqueID} not found, skipping.");
            CommandExecutionComplete();
            return;
        }
        
        var attackVisual = attacker.GetComponent<CreatureAttackVisual>();
        
        if (attackVisual == null)
        {
            Debug.LogError($"[CreatureAttackCommand] No CreatureAttackVisual on attacker, skipping.");
            CommandExecutionComplete();
            return;
        }

        // Apply health changes here — Die() calls happen now, inside StartCommandExecution,
        // so CreatureDieCommand always gets enqueued after this command is already running
        _applyDamage?.Invoke();
        
        attackVisual.AttackTarget(TargetUniqueID, DamageTakenByTarget, DamageTakenByAttacker, 
            AttackerHealthAfter, TargetHealthAfter);
    }
}