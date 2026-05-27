using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CardLogic: IIdentifiable
{
    [SerializeField, Tooltip("The player who owns this card")]
    private Player owner;
    [SerializeField, Tooltip("The unique ID of the card")]
    private int uniqueCardID; 
    [SerializeField, Tooltip("The CardAsset that contains the data for this card")]
    private CardAsset ca;
    
    private SpellEffect _effect;
    
    // STATIC (for managing IDs)
    public static Dictionary<int, CardLogic> CardsCreatedThisGame = new Dictionary<int, CardLogic>();

    #region Accessors
    public Player Owner { get => owner; set => owner = value; }
    public CardAsset CA => ca;
    public SpellEffect Effect => _effect;
    public int ID => uniqueCardID;
    public int CurrentManaCost{ get; set; }
    public bool CanBePlayed
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.WhoseTurn == owner);
            // for spells the amount of characters on the field does not matter
            bool fieldNotFull = true;
            // but if this is a creature, we have to check if there is room on board (table)
            if (ca.MaxHealth > 0)
                fieldNotFull = (owner.Table.CreaturesOnTable.Count < 7);
            //Debug.Log("Card: " + ca.name + " has params: ownersTurn=" + ownersTurn + "fieldNotFull=" + fieldNotFull + " hasMana=" + (CurrentManaCost <= owner.ManaLeft));
            return ownersTurn && fieldNotFull && (CurrentManaCost <= owner.ManaLeft);
        }
    }
    #endregion

    /// <summary>
    /// The constructor for CardLogic takes a CardAsset as a parameter and initializes the card's properties based on the data in the CardAsset. It also generates a unique ID for the card and creates an instance of the spell effect if specified in the CardAsset.
    /// Finally, it adds the card to a dictionary of all cards created during the game for easy reference.
    /// </summary>
    /// <param name="ca"></param>
    public CardLogic(CardAsset ca)
    {
        // set the CardAsset reference
        this.ca = ca;
        // get unique int ID
        uniqueCardID = IDFactory.GetUniqueID();
        ResetManaCost();
        // create an instance of SpellEffect with a name from our CardAsset
        // and attach it to 
        if (!string.IsNullOrEmpty(ca.SpellScriptName))
        {
            var type = Type.GetType(ca.SpellScriptName);
            if (type != null)
                _effect = (SpellEffect)Activator.CreateInstance(type);
        }
        // add this card to a dictionary with its ID as a key
        CardsCreatedThisGame.Add(uniqueCardID, this);
    }

    /// <summary>
    /// Resets the current mana cost of the card to its original mana cost as defined in the CardAsset. This is useful for effects that temporarily reduce the mana cost of a card, allowing it to be played for less mana for a turn or until a certain condition is met.
    /// Calling this method will restore the card's mana cost to its default value.
    /// </summary>
    public void ResetManaCost() => CurrentManaCost = ca.ManaCost;
 

}
