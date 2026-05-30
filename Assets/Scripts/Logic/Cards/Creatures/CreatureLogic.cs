using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureLogic: ICharacter
{
    [SerializeField, Tooltip("The owner of this card")]
    private Player owner;
    [SerializeField, Tooltip("The CardAsset that contains the data for this creature")]
    private CardAsset ca;
    [SerializeField, Tooltip("The ID of the creature that is used for this creature")]
    private int uniqueCreatureID;
    
    private CreatureEffect _effect;
    
    
    /// <summary>
    /// A dictionary that keeps track of all CreatureLogic instances created during the game, using their uniqueCreatureID as the key.
    /// This allows us to easily find a creature by its ID when we need to apply effects or handle attacks.
    /// </summary>
    public static Dictionary<int, CreatureLogic> CreaturesCreatedThisGame = new Dictionary<int, CreatureLogic>();
    
    // the basic health that we have in CardAsset
    private int _baseHealth;
    // current health of this creature
    private int _health;
    // property for Attack
    private int _baseAttack;
    // number of attacks for one turn if (attacksForOneTurn==2) => Windfury
    private int _attacksForOneTurn = 1;
    // Whether this creature is frozen or not
    private bool frozen = false;

    #region Accessors
    public Player Owner { get => owner; set => owner = value; }
    public CardAsset CA => ca;
    public CreatureEffect Effect => _effect;
    public bool Frozen => frozen;
    public int ID => uniqueCreatureID;
    public int MaxHealth => _baseHealth;
    public int Health
    {
        get => _health;

        set
        {
            if (value > MaxHealth)
                _health = MaxHealth;
            else if (value <= 0)
                Die();
            else
                _health = value;
        }
    }
    public int Attack => _baseAttack;
    public int AttacksLeftThisTurn { get; set; }
    /// <summary>
    /// Returns true if we can attack with this creature now.
    /// A creature can attack if it is the owner's turn, it has attacks left for this turn, and it is not frozen.
    /// </summary>
    public bool CanAttack
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.WhoseTurn == owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && !Frozen);
        }
    }
    #endregion

    /// <summary>
    /// Constructor for the CreatureLogic class.
    /// Initializes the creature's properties based on the provided CardAsset and owner.
    /// If the CardAsset has a special creature script, it creates an instance of that script and registers its event effects.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="ca"></param>
    public CreatureLogic(Player owner, CardAsset ca)
    {
        this.ca = ca;
        _baseHealth = ca.MaxHealth;
        Health = ca.MaxHealth;
        _baseAttack = ca.Attack;
        _attacksForOneTurn = ca.AttacksForOneTurn;
        // AttacksLeftThisTurn is now equal to 0
        if (ca.Charge)
            AttacksLeftThisTurn = _attacksForOneTurn;
        this.owner = owner;
        uniqueCreatureID = IDFactory.GetUniqueID();
        if (!string.IsNullOrEmpty(ca.CreatureScriptName))
        {
            var type = System.Type.GetType(ca.CreatureScriptName);
            if (type != null)
                _effect = (CreatureEffect)System.Activator.CreateInstance(
                    type, owner, this, ca.SpecialCreatureAmount);
            if (_effect != null) 
                _effect.RegisterEventEffect();
        }
        CreaturesCreatedThisGame.Add(uniqueCreatureID, this);
    }

    #region Logic Methods
    /// <summary>
    /// Resets the number of attacks left for this creature at the start of the owner's turn.
    /// </summary>
    public void OnTurnStart() => AttacksLeftThisTurn = _attacksForOneTurn;
    
    /// <summary>
    /// Handles the death of this creature.
    /// Removes it from the owner's table, triggers any deathrattle effects, and sends a command to update the game state accordingly.
    /// </summary>
    public void Die()
    {   
        owner.Table.CreaturesOnTable.Remove(this);

        // cause Deathrattle Effect
        if (_effect != null)
        {
            _effect.WhenACreatureDies();
            _effect.UnRegisterEventEffect();
            _effect = null;
        }

        new CreatureDieCommand(uniqueCreatureID, owner).AddToQueue();
    }

    /// <summary>
    /// Handles attacking another creature with this creature.
    /// </summary>
    /// <param name="target"></param>
    public void AttackCreature(CreatureLogic target)
    {
        AttacksLeftThisTurn--;
    
        int targetHealthAfter = target.Health - Attack;
        int attackerHealthAfter = Health - target.Attack;
    
        new CreatureAttackCommand(
            target.ID, 
            uniqueCreatureID,
            target.Attack,    
            Attack,           
            attackerHealthAfter, 
            targetHealthAfter,
            () =>
            {
                target.Health -= Attack;
                Health -= target.Attack;
            }
        ).AddToQueue();
    }

    /// <summary>
    /// Handles attacking the opponent's face with this creature.
    /// </summary>
    public void GoFace()
    {
        AttacksLeftThisTurn--;
        int targetHealthAfter = owner.OtherPlayer.Health - Attack;

        new CreatureAttackCommand(
            owner.OtherPlayer.ID, 
            uniqueCreatureID,
            0, 
            Attack, 
            Health,           
            targetHealthAfter,
            () => owner.OtherPlayer.Health -= Attack
        ).AddToQueue();
    }

    /// <summary>
    /// Handles attacking another creature by its unique ID.
    /// </summary>
    /// <param name="uniqueCreatureID"></param>
    public void AttackCreatureWithID(int uniqueCreatureID)
    {
        CreatureLogic target = CreaturesCreatedThisGame[uniqueCreatureID];
        AttackCreature(target);
    }
    #endregion
}
