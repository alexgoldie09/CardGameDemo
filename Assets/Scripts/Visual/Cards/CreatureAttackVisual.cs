using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CreatureAttackVisual : MonoBehaviour 
{
    private CreatureInfoManager _manager;
    private WhereIsTheCardOrCreature _w;

    /// <summary>
    /// Gets the creature info manager and where is the card or creature component of this creature.
    /// </summary>
    private void Awake()
    {
        _manager = GetComponent<CreatureInfoManager>();    
        _w = GetComponent<WhereIsTheCardOrCreature>();
    }

    /// <summary>
    /// Animates the attack of this creature to the target.
    /// Also updates health text of both attacker and target and creates damage effect if needed.
    /// </summary>
    /// <param name="targetUniqueID"></param>
    /// <param name="damageTakenByTarget"></param>
    /// <param name="damageTakenByAttacker"></param>
    /// <param name="attackerHealthAfter"></param>
    /// <param name="targetHealthAfter"></param>
    public void AttackTarget(int targetUniqueID, int damageTakenByTarget, int damageTakenByAttacker, int attackerHealthAfter, int targetHealthAfter)
    {
        Debug.Log(targetUniqueID);
        _manager.CanAttackNow = false;
        GameObject target = IDHolder.GetGameObjectWithID(targetUniqueID);

        // bring this creature to front sorting-wise.
        _w.BringToFront();
        VisualStates tempState = _w.VisualState;
        _w.VisualState = VisualStates.Transition;

        transform.DOMove(target.transform.position, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if(damageTakenByTarget>0)
                    DamageEffect.CreateDamageEffect(target.transform.position, damageTakenByTarget);
                if(damageTakenByAttacker>0)
                    DamageEffect.CreateDamageEffect(transform.position, damageTakenByAttacker);
                
                if (targetUniqueID == GlobalSettings.Instance.LowPlayer.ID || targetUniqueID == GlobalSettings.Instance.TopPlayer.ID)
                {
                    // target is a player
                    target.GetComponent<PlayerPortraitVisual>().HealthText.text = targetHealthAfter.ToString();
                }
                else
                    target.GetComponent<CreatureInfoManager>().UpdateHealthDisplay(targetHealthAfter);

                _w.SetTableSortingOrder();
                _w.VisualState = tempState;

                _manager.HealthText.text = attackerHealthAfter.ToString();
                Sequence s = DOTween.Sequence();
                s.AppendInterval(1f);
                s.OnComplete(Command.CommandExecutionComplete);
                //Command.CommandExecutionComplete();
            });
    }
        
}
