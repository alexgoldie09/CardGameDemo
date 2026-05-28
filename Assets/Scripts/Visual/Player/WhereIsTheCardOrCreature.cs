using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// an enum to store the info about where this object is
public enum VisualStates
{
    Transition,
    LowHand, 
    TopHand,
    LowTable,
    TopTable,
    Dragging
}

public class WhereIsTheCardOrCreature : MonoBehaviour {

    // reference to a HoverPreview Component
    private HoverPreview _hover;

    // reference to a canvas on this object to set sorting order
    private Canvas _canvas;

    // a value for canvas sorting order when we want to show this object above everything
    private int _topSortingOrder = 500;
    
    private int _slot = -1;
    
    private VisualStates _state;
    
    #region Properties
    /// <summary>
    /// The slot index of this card in the player's hand.
    /// </summary>
    public int Slot
    {
        get => _slot;
        set => _slot = value;
    }
    
    /// <summary>
    /// The current visual state of this card or creature, which determines
    /// how it is displayed and whether the hover preview is enabled.
    /// </summary>
    public VisualStates VisualState
    {
        get => _state;

        set
        {
            _state = value;
            _hover.ThisPreviewEnabled = _state switch
            {
                VisualStates.LowHand or VisualStates.TopHand => true,
                VisualStates.LowTable or VisualStates.TopTable => true,
                VisualStates.Transition => false,
                VisualStates.Dragging => false,
                _ => _hover.ThisPreviewEnabled
            };
        }
    }
    #endregion

    /// <summary>
    /// Initializes references to the HoverPreview component and the Canvas component on this object.
    /// </summary>
    private void Awake()
    {
        _hover = GetComponent<HoverPreview>();
        // for characters hover is attached to a child game object
        if (_hover == null)
            _hover = GetComponentInChildren<HoverPreview>();
        _canvas = GetComponentInChildren<Canvas>();
    }

    /// <summary>
    /// Sets the canvas sorting order and layer to bring this card or creature to the front of the display,
    /// ensuring it appears above all other elements in the scene.
    /// </summary>
    public void BringToFront()
    {
        _canvas.sortingOrder = _topSortingOrder;
        _canvas.sortingLayerName = "AboveEverything";
    }

    // not setting sorting order inside of VisualStaes property because when the card is drawn, 
    // we want to set an index first and set the sorting order only when the card arrives to hand. 
    public void SetHandSortingOrder()
    {
        if (_slot != -1)
            _canvas.sortingOrder = HandSortingOrder(_slot);
        _canvas.sortingLayerName = "Cards";
    }

    /// <summary>
    /// Sets the canvas sorting order and layer for a card or creature that is on the table,
    /// ensuring it appears in the correct order relative to other table elements.
    /// </summary>
    public void SetTableSortingOrder()
    {
        _canvas.sortingOrder = 0;
        _canvas.sortingLayerName = "Creatures";
    }

    /// <summary>
    /// Calculates the sorting order for a card in the player's hand based on its slot index.
    /// </summary>
    /// <param name="placeInHand"></param>
    /// <returns></returns>
    private int HandSortingOrder(int placeInHand)
    {
        return (-(placeInHand + 1) * 10); 
    }
}
