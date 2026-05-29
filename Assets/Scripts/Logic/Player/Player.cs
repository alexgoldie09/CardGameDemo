using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour, ICharacter
{
    [Header("Player")]
    [SerializeField, Tooltip("The character asset that contains all the data about this hero. Assign it in inspector.")]
    private CharacterAsset charAsset;
    [SerializeField, Tooltip("Reference to the player area that belongs to this player. Assign it in inspector.")]
    private PlayerArea pArea;
    [SerializeField, Tooltip("The script of type SpellEffect that will be used for this hero's hero power. Assign it in inspector.")]
    private SpellEffect heroPowerEffect;
    
    /// <summary>
    /// A unique identifier for this player instance. It is assigned using an IDFactory to ensure uniqueness across the game.
    /// </summary>
    private int playerID;
    /// <summary>
    /// A boolean flag to track whether the player has used their hero power this turn.
    /// This is used to prevent the player from using the hero power more than once per turn, as per game rules.
    /// </summary>
    private bool usedHeroPowerThisTurn = false;

    [Header("Cards")]
    [SerializeField, Tooltip("The deck of this player. Assign it in inspector.")]
    private Deck deck;
    [SerializeField, Tooltip("The cards in the hand of this player.")]
    private Hand hand;
    [SerializeField, Tooltip("The cards on the table of this player.")]
    private Table table;

    // a static array that will store both players, should always have 2 players
    public static Player[] Players;

    // this value used exclusively for our coin spell
    private int _bonusManaThisTurn = 0;
    
    // total mana crystals that this player has this turn
    private int _manaThisTurn;
    
    // full mana crystals available right now to play cards / use hero power
    private int _manaLeft;
    
    // health for the player
    private int _health;

    #region Accessors
    public int ID => playerID;
    public CharacterAsset CharAsset => charAsset;
    public PlayerArea PArea => pArea;
    public SpellEffect HeroPowerEffect => heroPowerEffect;
    public bool UsedHeroPowerThisTurn => usedHeroPowerThisTurn;
    public Deck Deck => deck;
    public Hand Hand => hand;
    public Table Table => table;
    #endregion

    #region Properties
    /// <summary>
    /// A property to get the other player in the game.
    /// It checks the static Players array and returns the player that is not this instance.
    /// </summary>
    public Player OtherPlayer
    {
        get
        {
            if (Players[0] == this)
                return Players[1];
            
            return Players[0];
        }
    }
    
    /// <summary>
    /// The total number of mana crystals that this player has for the current turn.
    /// </summary>
    public int ManaThisTurn
    {
        get => _manaThisTurn;
        set
        {
            if (value < 0)
                _manaThisTurn = 0;
            else if (value > PArea.ManaBar.Crystals.Length)
                _manaThisTurn = PArea.ManaBar.Crystals.Length;
            else
                _manaThisTurn = value;
            new UpdateManaCrystalsCommand(this, _manaThisTurn, _manaLeft).AddToQueue();
        }
    }
    
    /// <summary>
    /// The number of mana crystals that this player currently has available to spend on playing cards or using hero power.
    /// </summary>
    public int ManaLeft
    {
        get => _manaLeft;
        set
        {
            if (value < 0)
                _manaLeft = 0;
            else if (value > PArea.ManaBar.Crystals.Length)
                _manaLeft = PArea.ManaBar.Crystals.Length;
            else
                _manaLeft = value;

            new UpdateManaCrystalsCommand(this, ManaThisTurn, _manaLeft).AddToQueue();
            if (TurnManager.Instance.WhoseTurn == this)
                HighlightPlayableCards();
        }
    }
    
    /// <summary>
    /// The current health of the player's hero.
    /// Setting this property will ensure that the health does not exceed the maximum health defined in the character asset, and will trigger the Die() method if health drops to 0 or below.
    /// </summary>
    public int Health
    {
        get => _health;
        set
        {
            if (value > charAsset.MaxHealth)
                _health = charAsset.MaxHealth;
            else
                _health = value;
            if (value <= 0)
                Die();
        }
    }
    #endregion

    // EVENTS
    //public event Action OnCreaturePlayed;
    //public event Action OnSpellPlayed;
    //public event Action OnTurnStarted;
    public event Action OnEndTurn;
    
    private void Awake()
    {
        Players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        playerID = IDFactory.GetUniqueID();
    }
    
    #region Turn Methods
    public virtual void OnTurnStart()
    {
        Debug.Log("In ONTURNSTART for " + gameObject.name);
        usedHeroPowerThisTurn = false;
        ManaThisTurn++;
        ManaLeft = ManaThisTurn;
        foreach (CreatureLogic cl in table.CreaturesOnTable)
            cl.OnTurnStart();
        PArea.HeroPower.WasUsedThisTurn = false;
    }

    public void OnTurnEnd()
    {
        OnEndTurn?.Invoke();
        ManaThisTurn -= _bonusManaThisTurn;
        _bonusManaThisTurn = 0;
        GetComponent<TurnMaker>().StopAllCoroutines();
    }
    #endregion

   #region Player Methods
   /// <summary>
   /// A method to add bonus mana for the current turn.
   /// This can be used for effects like the coin spell that gives you extra mana for one turn.
   /// It increases both the total mana crystals for this turn and the mana left to spend, and keeps track of how much bonus mana was added so it can be removed at the end of the turn.
   /// </summary>
   /// <param name="amount"></param>
   public void GetBonusMana(int amount)
   {
       _bonusManaThisTurn += amount;
       ManaThisTurn += amount;
       ManaLeft += amount;
   }

    // FOR TESTING ONLY
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.D))
    //         DrawACard();
    // }

    /// <summary>
    /// A method to draw a card from the player's deck into their hand.
    /// It checks if there are cards left in the deck and if there is space in the hand
    /// (not exceeding the maximum hand size defined by the number of hand slots in the player area).
    /// </summary>
    /// <param name="fast"></param>
    public void DrawACard(bool fast = false)
    {
        if (deck.Cards.Count > 0)
        {
            if (hand.CardsInHand.Count < PArea.HandVisual.Slots.Children.Length)
            {
                // 1) logic: add card to hand
                var newCard = new CardLogic(deck.Cards[0])
                {
                    Owner = this
                };
                hand.CardsInHand.Insert(0, newCard);
                // 2) logic: remove the card from the deck
                deck.Cards.RemoveAt(0);
                // 3) create a command
                new DrawACardCommand(hand.CardsInHand[0], this, fast, fromDeck: true).AddToQueue();
            }
        }
        else
        {
            // there are no cards in the deck, take fatigue damage.
        }
    }

    /// <summary>
    /// A method to add a card to the player's hand that is not drawn from the deck.
    /// This can be used for effects that generate cards directly into the hand,
    /// such as the coin spell or certain hero powers or card effects.
    /// It checks if there is space in the hand before adding the card,
    /// but it does not remove the card from the deck since it was not drawn from there.
    /// </summary>
    /// <param name="cardAsset"></param>
    public void GetACardNotFromDeck(CardAsset cardAsset)
    {
        if (hand.CardsInHand.Count < PArea.HandVisual.Slots.Children.Length)
        {
            // 1) logic: add card to hand
            var newCard = new CardLogic(cardAsset)
            {
                Owner = this
            };
            hand.CardsInHand.Insert(0, newCard);
            // 2) send message to the visual Deck
            new DrawACardCommand(hand.CardsInHand[0], this, fast: true, fromDeck: false).AddToQueue();
        }
        // no removal from deck because the card was not in the deck
    }

    // 2 METHODS FOR PLAYING SPELLS
    // 1st overload - takes ids as arguments
    // it is convenient to call this method from visual part
    public void PlayASpellFromHand(int spellCardUniqueID, int targetUniqueID)
    {
        if (targetUniqueID < 0)
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[spellCardUniqueID], null);
        else if (targetUniqueID == ID)
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[spellCardUniqueID], this);
        else if (targetUniqueID == OtherPlayer.ID)
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[spellCardUniqueID], this.OtherPlayer);
        else
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[spellCardUniqueID], CreatureLogic.CreaturesCreatedThisGame[targetUniqueID]);
    }

    // 2nd overload - takes CardLogic and ICharacter interface
    // this method is called from Logic, for example by AI
    public void PlayASpellFromHand(CardLogic playedCard, ICharacter target)
    {
        ManaLeft -= playedCard.CurrentManaCost;
        if (playedCard.Effect != null)
            playedCard.Effect.ActivateEffect(playedCard.CA.SpecialSpellAmount, target);
        else
            Debug.LogWarning("No effect found on card " + playedCard.CA.name);

        new PlayASpellCardCommand(this, playedCard).AddToQueue();
        hand.CardsInHand.Remove(playedCard);
    }

    // METHODS TO PLAY CREATURES
    // 1st overload - by ID
    public void PlayACreatureFromHand(int uniqueID, int tablePos) =>
        PlayACreatureFromHand(CardLogic.CardsCreatedThisGame[uniqueID], tablePos);

    // 2nd overload - by logic units
    public void PlayACreatureFromHand(CardLogic playedCard, int tablePos)
    {
        ManaLeft -= playedCard.CurrentManaCost;
        var newCreature = new CreatureLogic(this, playedCard.CA);
        table.CreaturesOnTable.Insert(tablePos, newCreature);
        new PlayACreatureCommand(playedCard, this, tablePos, newCreature.ID).AddToQueue();
        if (newCreature.Effect != null)
            newCreature.Effect.WhenACreatureIsPlayed();
        hand.CardsInHand.Remove(playedCard);
        HighlightPlayableCards();
    }
    
    /// <summary>
    /// A method to use the player's hero power.
    /// It checks if the hero power has already been used this turn and if the
    /// player has enough mana to use it. If the hero power can be used, it reduces the player's mana
    /// by the cost of the hero power (usually 2), sets the flag to indicate that the hero power
    /// has been used this turn, and activates the hero power's effect.
    /// </summary>
    public void UseHeroPower()
    {
        ManaLeft -= charAsset.HeroPowerManaCost;
        usedHeroPowerThisTurn = true;
        HeroPowerEffect.ActivateEffect();
    }
    #endregion

    /// <summary>
    /// A method to handle the death of the player's hero.
    /// It disables controls for both players, stops the turn timer, and sends a command to
    /// trigger the game over sequence with this player as the loser.
    /// </summary>
    public void Die()
    {
        PArea.ControlsOn = false;
        OtherPlayer.PArea.ControlsOn = false;
        TurnManager.Instance.StopTheTimer();
        new GameOverCommand(this).AddToQueue();
    }

    #region Glow Highlight Methods
    /// <summary>
    /// A method to highlight the cards in the player's hand that can currently be played,
    /// as well as the creatures on the table that can attack, and the hero power if it can be used.
    /// </summary>
    /// <param name="removeAllHighlights"></param>
    public void HighlightPlayableCards(bool removeAllHighlights = false)
    {
        foreach (var cl in hand.CardsInHand)
        {
            var g = IDHolder.GetGameObjectWithID(cl.ID);
            if (g != null)
                g.GetComponent<CardInfoManager>().CanBePlayedNow = (cl.CurrentManaCost <= ManaLeft) && !removeAllHighlights;
        }

        foreach (var crl in table.CreaturesOnTable)
        {
            var g = IDHolder.GetGameObjectWithID(crl.ID);
            if (g != null)
                g.GetComponent<CreatureInfoManager>().CanAttackNow = (crl.AttacksLeftThisTurn > 0) && !removeAllHighlights;
        }

        PArea.HeroPower.Highlighted = (!usedHeroPowerThisTurn) && (ManaLeft >= charAsset.HeroPowerManaCost) && !removeAllHighlights;
        
    }
    #endregion

    #region Start Game Methods
    /// <summary>
    /// A method to load the character information from the assigned CharacterAsset at the start of the game.
    /// </summary>
    public void LoadCharacterInfoFromAsset()
    {
        Health = charAsset.MaxHealth;
        PArea.Portrait.CharAsset = charAsset;

        if (!string.IsNullOrEmpty(charAsset.HeroPowerName) && heroPowerEffect == null)
        {
            var type = Type.GetType(charAsset.HeroPowerName);
            if (type != null)
                heroPowerEffect = (SpellEffect)Activator.CreateInstance(type);
        }
        else
            Debug.LogWarning("Check hero power name for character " + charAsset.ClassName);
    }

    public void TransmitInfoAboutPlayerToVisual()
    {
        PArea.Portrait.gameObject.AddComponent<IDHolder>().UniqueID = ID;
        if (GetComponent<TurnMaker>() is AITurnMaker)
            PArea.AllowedToControlThisPlayer = false;
        else
            PArea.AllowedToControlThisPlayer = true;
    }
    #endregion
}