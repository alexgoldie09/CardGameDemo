using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HandVisual : MonoBehaviour
{
    [Header("Ownership / Behavior")]
    [SerializeField, Tooltip("Which side this hand belongs to (Top or Low).")]
    private AreaPosition owner;
    [SerializeField, Tooltip("If true, drawn cards are revealed face-up during draw animation.")]
    private bool takeCardsOpenly = true;
    [SerializeField, Tooltip("Slot container that controls spacing/positions of cards in hand.")]
    private SameDistanceChildren slots;
        
    [Header("Transform References")]
    [SerializeField, Tooltip("World position where a drawn card is previewed before entering hand.")]
    private Transform drawPreviewSpot;
    [SerializeField, Tooltip("World position used as the card source when drawing from deck.")]
    private Transform deckTransform;
    [SerializeField, Tooltip("Alternative card source when card is not drawn from deck.")]
    private Transform otherCardDrawSourceTransform;
    [SerializeField, Tooltip("World position where played spells are previewed.")]
    private Transform playPreviewSpot;

    #region Accessors
    public AreaPosition Owner => owner;
    public bool TakeCardsOpenly => takeCardsOpenly;
    public SameDistanceChildren Slots => slots;
    public Transform DrawPreviewSpot => drawPreviewSpot;
    public Transform DeckTransform => deckTransform;
    public Transform OtherCardDrawSourceTransform => otherCardDrawSourceTransform;
    public Transform PlayPreviewSpot => playPreviewSpot;
    #endregion
    
    // a list of all card visual representations as GameObjects
    private List<GameObject> CardsInHand = new List<GameObject>();

    #region Adding and Removing Cards
    /// <summary>
    /// Adds a card GameObject to the player's hand, parents it to the slots container, and updates card positions.
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(GameObject card)
    {
        // we allways insert a new card as 0th element in CardsInHand List 
        CardsInHand.Insert(0, card);

        // parent this card to our Slots GameObject
        card.transform.SetParent(slots.transform);

        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    /// <summary>
    /// Removes a card GameObject from the player's hand and updates card positions.
    /// </summary>
    /// <param name="card"></param>
    public void RemoveCard(GameObject card)
    {
        // remove a card from the list
        CardsInHand.Remove(card);

        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    /// <summary>
    /// Removes a card at a specific index from the player's hand and updates card positions.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveCardAtIndex(int index)
    {
        CardsInHand.RemoveAt(index);
        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    /// <summary>
    /// Retrieves the card GameObject at a specific index in the player's hand.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetCardAtIndex(int index) => CardsInHand[index];

    #endregion
        
    #region Managing Cards and Slots
    /// <summary>
    /// Updates the position of the slots container based on the number of cards in hand,
    /// centering it if there are cards or resetting to default if empty.
    /// </summary>
    private void UpdatePlacementOfSlots()
    {
        float posX;
        if (CardsInHand.Count > 0)
            posX = (slots.Children[0].transform.localPosition.x - slots.Children[CardsInHand.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        // tween Slots GameObject to new position in 0.3 seconds
        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);  
    }

    /// <summary>
    /// Animates all cards in hand to move to their new slot positions based on their index in the CardsInHand list,
    /// and updates their sorting order and slot index for correct display and interaction.
    /// </summary>
    private void PlaceCardsOnNewSlots()
    {
        foreach (var g in CardsInHand)
        {
            // tween this card to a new Slot
            g.transform.DOLocalMoveX(slots.Children[CardsInHand.IndexOf(g)].transform.localPosition.x, 0.3f);

            // apply correct sorting order and HandSlot value for later 
            WhereIsTheCardOrCreature w = g.GetComponent<WhereIsTheCardOrCreature>();
            w.Slot = CardsInHand.IndexOf(g);
            w.SetHandSortingOrder();
        }
    }
    #endregion
    
    
    #region Card Draw Methods
    /// <summary>
    /// Creates a card GameObject at a specified position and rotation based on the provided CardAsset information,
    /// instantiating the appropriate prefab for creature or spell cards and applying the card's visual details.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="position"></param>
    /// <param name="eulerAngles"></param>
    /// <returns></returns>
    public GameObject CreateACardAtPosition(CardAsset c, Vector3 position, Vector3 eulerAngles)
    {
        // Instantiate a card depending on its type
        GameObject card;
        if (c.MaxHealth > 0)
        {
            // this card is a creature card
            card = Instantiate(GlobalSettings.Instance.CreatureCardPrefab, position, Quaternion.Euler(eulerAngles));
        }
        else
        {
            // this is a spell: checking for targeted or non-targeted spell
            if (c.Targets == TargetingOptions.NoTarget)
                card = Instantiate(GlobalSettings.Instance.NoTargetSpellCardPrefab, position, Quaternion.Euler(eulerAngles));
            else
            {
                card = Instantiate(GlobalSettings.Instance.TargetedSpellCardPrefab, position, Quaternion.Euler(eulerAngles));
                // pass targeting options to DraggingActions
                DragSpellOnTarget dragSpell = card.GetComponentInChildren<DragSpellOnTarget>();
                dragSpell.Targets = c.Targets;
            }

        }

        // apply the look of the card based on the info from CardAsset
        CardInfoManager manager = card.GetComponent<CardInfoManager>();
        manager.LoadCard(c);

        return card;
    }
    
    /// <summary>
    /// Animates the process of giving a card to the player, including creating the card GameObject,
    /// setting its initial position and rotation, adding it to the hand, and moving it to the hand
    /// with a transition animation. The method also handles setting the card's visual state and sorting order
    /// once it arrives in the hand, and completes the command execution for drawing a card.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="uniqueID"></param>
    /// <param name="fast"></param>
    /// <param name="fromDeck"></param>
    public void GivePlayerACard(CardAsset c, int uniqueID, bool fast = false, bool fromDeck = true)
    {
        var card = CreateACardAtPosition(c, fromDeck ? DeckTransform.position : OtherCardDrawSourceTransform.position, new Vector3(0f, -179f, 0f));

        // Set a tag to reflect where this card is
        foreach (Transform t in card.GetComponentsInChildren<Transform>())
            t.tag = $"{owner}Card";
        // pass this card to HandVisual class
        AddCard(card);

        // Bring card to front while it travels from draw spot to hand
        WhereIsTheCardOrCreature w = card.GetComponent<WhereIsTheCardOrCreature>();
        w.BringToFront();
        w.Slot = 0; 
        w.VisualState = VisualStates.Transition;

        // pass a unique ID to this card.
        IDHolder id = card.AddComponent<IDHolder>();
        id.UniqueID = uniqueID;

        // move card to the hand;
        Sequence s = DOTween.Sequence();
        if (!fast)
        {
            // Debug.Log ("Not fast!!!");
            s.Append(card.transform.DOMove(DrawPreviewSpot.position, GlobalSettings.Instance.CardTransitionTime));
            if (TakeCardsOpenly)
                s.Insert(0f, card.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTime)); 
            //else 
                //s.Insert(0f, card.transform.DORotate(new Vector3(0f, -179f, 0f), GlobalSettings.Instance.CardTransitionTime)); 
            s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
            // displace the card so that we can select it in the scene easier.
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GlobalSettings.Instance.CardTransitionTime));
        }
        else
        {
            // displace the card so that we can select it in the scene easier.
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GlobalSettings.Instance.CardTransitionTimeFast));
            if (TakeCardsOpenly)    
                s.Insert(0f,card.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTimeFast)); 
        }

        s.OnComplete(()=>ChangeLastCardStatusToInHand(card, w));
    }

    /// <summary>
    /// Sets the visual state of the card to the appropriate hand state (LowHand or TopHand) based on ownership,
    /// updates the card's sorting order for correct display in the hand, and signals that the command execution
    /// for drawing a card is complete.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="w"></param>
    void ChangeLastCardStatusToInHand(GameObject card, WhereIsTheCardOrCreature w)
    {
        //Debug.Log("Changing state to Hand for card: " + card.gameObject.name);
        if (owner == AreaPosition.Low)
            w.VisualState = VisualStates.LowHand;
        else
            w.VisualState = VisualStates.TopHand;

        // set correct sorting order
        w.SetHandSortingOrder();
        // end command execution for DrawACArdCommand
        Command.CommandExecutionComplete();
    }
    #endregion
   
    #region Playing Spell Methods
    public void PlayASpellFromHand(int CardID)
    {
        GameObject card = IDHolder.GetGameObjectWithID(CardID);
        PlayASpellFromHand(card);
    }
    
    /// <summary>
    /// Animates the process of playing a spell card from the player's hand, including moving the card to a preview spot,
    /// rotating it to a neutral orientation, and then destroying the card GameObject after a delay.
    /// The method also signals the completion of the command execution for playing a spell.
    /// </summary>
    /// <param name="CardVisual"></param>
    public void PlayASpellFromHand(GameObject CardVisual)
    {
        Command.CommandExecutionComplete();
        CardVisual.GetComponent<WhereIsTheCardOrCreature>().VisualState = VisualStates.Transition;
        RemoveCard(CardVisual);

        CardVisual.transform.SetParent(null);

        Sequence s = DOTween.Sequence();
        s.Append(CardVisual.transform.DOMove(PlayPreviewSpot.position, 1f));
        s.Insert(0f, CardVisual.transform.DORotate(Vector3.zero, 1f));
        s.AppendInterval(2f);
        s.OnComplete(()=>
            {
                //Command.CommandExecutionComplete();
                Destroy(CardVisual);
            });
    }
    #endregion

}
