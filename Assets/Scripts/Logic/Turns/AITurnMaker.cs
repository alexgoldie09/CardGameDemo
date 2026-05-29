using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this class will take all decisions for AI. 

public class AITurnMaker: TurnMaker 
{
    /// <summary>
    /// This method is called at the start of the AI`s turn.
    /// It will display a message that it is enemy`s turn and draw a card for the AI.
    /// </summary>
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        // dispay a message that it is enemy`s turn
        new ShowMessageCommand("Enemy`s Turn!", 2.0f).AddToQueue();
        p.DrawACard();
        StartCoroutine(MakeAITurn());
    }

    #region AI Logic
    /// <summary>
    /// This method will make the AI take its turn by making a series of moves until it can no longer make any moves.
    /// The AI will randomly decide whether to prioritize attacking with creatures or playing cards from hand first.
    /// After each move, there will be a short delay to make the AI`s actions more visually appealing and easier to follow for the player.
    /// </summary>
    /// <returns></returns>
    IEnumerator MakeAITurn()
    {
        bool strategyAttackFirst = Random.Range(0, 2) == 0;

        while (MakeOneAIMove(strategyAttackFirst))
        {
            yield return null;
        }

        InsertDelay(1f);

        TurnManager.Instance.EndTurn();
    }

    /// <summary>
    /// This method will make one move for the AI, based on the chosen strategy (attack first or play cards first).
    /// </summary>
    /// <param name="attackFirst"></param>
    /// <returns></returns>
    private bool MakeOneAIMove(bool attackFirst)
    {
        if (Command.CardDrawPending())
            return true;
        
        if (attackFirst)
            return AttackWithACreature() || PlayACardFromHand() || UseHeroPower();
        
        return PlayACardFromHand() || AttackWithACreature() || UseHeroPower();
    }

    /// <summary>
    /// This method will attempt to play a card from the AI`s hand.
    /// It will check each card in hand to see if it can be played, and if so, it will play the first playable card it finds.
    /// If the card is a spell, it will also check the targeting options and select a random target if necessary.
    /// </summary>
    /// <returns></returns>
    private bool PlayACardFromHand()
    {
        foreach (CardLogic c in p.Hand.CardsInHand)
        {
            if (c.CanBePlayed)
            {
                if (c.CA.MaxHealth == 0)
                {
                    // code to play a spell from hand
                    // depending on the targeting options, select a random target.
                    if (c.CA.Targets == TargetingOptions.NoTarget)
                    {
                        p.PlayASpellFromHand(c, null);
                        InsertDelay(1.5f);
                        //Debug.Log("Card: " + c.ca.name + " can be played");
                        return true;
                    }
                    else
                    {
                        ICharacter target = SelectRandomTarget(c.CA.Targets);
                        if (target != null)
                        {
                            p.PlayASpellFromHand(c, target);
                            InsertDelay(1.5f);
                            return true;
                        }
                    }
                }
                else
                {
                    // it is a creature card
                    p.PlayACreatureFromHand(c, 0);
                    InsertDelay(1.5f);
                    return true;
                }

            }
            //Debug.Log("Card: " + c.ca.name + " can NOT be played");
        }
        return false;
    }

    /// <summary>
    /// This method will attempt to use the AI`s hero power.
    /// It will check if the AI has enough mana to use the hero power and if it has not already used it this turn.
    /// If both conditions are met, it will use the hero power and return true. Otherwise, it will return false.
    /// </summary>
    /// <returns></returns>
    private bool UseHeroPower()
    {
        if (p.ManaLeft >= 2 && !p.UsedHeroPowerThisTurn)
        {
            // use HP
            p.UseHeroPower();
            InsertDelay(1.5f);
            //Debug.Log("AI used hero power");
            return true;
        }
        return false;
    }

    /// <summary>
    /// This method will attempt to attack with one of the AI`s creatures.
    /// It will check each creature on the AI`s table to see if it has any attacks left
    /// for this turn, and if so, it will attack a random target (either an enemy creature or the enemy hero).
    /// </summary>
    /// <returns></returns>
    private bool AttackWithACreature()
    {
        foreach (var cl in p.Table.CreaturesOnTable)
        {
            if (cl.AttacksLeftThisTurn > 0)
            {
                // attack a random target with a creature
                if (p.OtherPlayer.Table.CreaturesOnTable.Count > 0)
                {
                    int index = Random.Range(0, p.OtherPlayer.Table.CreaturesOnTable.Count);
                    CreatureLogic targetCreature = p.OtherPlayer.Table.CreaturesOnTable[index];
                    cl.AttackCreature(targetCreature);
                }                    
                else
                    cl.GoFace();
                
                InsertDelay(1f);
                //Debug.Log("AI attacked with creature");
                return true;
            }
        }
        return false;
    }
    #endregion

    /// <summary>
    /// This method will insert a delay in the AI`s actions by adding a DelayCommand to the command queue.
    /// </summary>
    /// <param name="delay"></param>
    private void InsertDelay(float delay)
    {
        new DelayCommand(delay).AddToQueue();
    }
    
    /// <summary>
    /// Returns a random valid ICharacter target for the given targeting option, or null if none exist.
    /// </summary>
    private ICharacter SelectRandomTarget(TargetingOptions targeting)
    {
        List<ICharacter> valid = new List<ICharacter>();

        switch (targeting)
        {
            case TargetingOptions.EnemyCreatures:
                foreach (var cl in p.OtherPlayer.Table.CreaturesOnTable) valid.Add(cl);
                break;
            case TargetingOptions.YourCreatures:
                foreach (var cl in p.Table.CreaturesOnTable) valid.Add(cl);
                break;
            case TargetingOptions.AllCreatures:
                foreach (var cl in p.Table.CreaturesOnTable) valid.Add(cl);
                foreach (var cl in p.OtherPlayer.Table.CreaturesOnTable) valid.Add(cl);
                break;
            case TargetingOptions.EnemyCharacters:
                foreach (var cl in p.OtherPlayer.Table.CreaturesOnTable) valid.Add(cl);
                valid.Add(p.OtherPlayer);
                break;
            case TargetingOptions.YourCharacters:
                foreach (var cl in p.Table.CreaturesOnTable) valid.Add(cl);
                valid.Add(p);
                break;
            case TargetingOptions.AllCharacters:
                foreach (var cl in p.Table.CreaturesOnTable) valid.Add(cl);
                foreach (var cl in p.OtherPlayer.Table.CreaturesOnTable) valid.Add(cl);
                valid.Add(p);
                valid.Add(p.OtherPlayer);
                break;
        }

        return valid.Count > 0 ? valid[Random.Range(0, valid.Count)] : null;
    }

}
