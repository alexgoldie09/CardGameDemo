using System;
using UnityEngine;
using DG.Tweening;

public class DragCreatureOnTable : DraggingActions
{
    private int _savedHandSlot;
    private VisualStates _tempState;
    private WhereIsTheCardOrCreature _whereIsCard;
    private CardInfoManager _manager;

    public override bool CanDrag => base.CanDrag && _manager.CanBePlayedNow;

    private void Awake()
    {
        _whereIsCard = GetComponent<WhereIsTheCardOrCreature>();
        _manager = GetComponent<CardInfoManager>();
    }

    public override void OnStartDrag()
    {
        _savedHandSlot = _whereIsCard.Slot;
        _tempState = _whereIsCard.VisualState;
        _whereIsCard.VisualState = VisualStates.Dragging;
        _whereIsCard.BringToFront();
    }

    public override void OnDraggingInUpdate() { }

    /// <summary>
    /// Plays the creature if dropped over the table, otherwise animates it back to its hand slot.
    /// CardInputManager already moves the card to the pointer position so transform.position.x
    /// is used directly rather than reading Input.mousePosition.
    /// </summary>
    public override void OnEndDrag(Action onComplete = null)
    {
        if (DragSuccessful())
        {
            int tablePos = PlayerOwner.PArea.TableVisual.TablePosForNewCreature(transform.position.x);
            PlayerOwner.PlayACreatureFromHand(GetComponent<IDHolder>().UniqueID, tablePos);
            onComplete?.Invoke();
        }
        else
        {
            _whereIsCard.SetHandSortingOrder();
            _whereIsCard.VisualState = _tempState;

            Vector3 oldCardPos = PlayerOwner.PArea.HandVisual.Slots.Children[_savedHandSlot].transform.localPosition;
            transform.DOLocalMove(oldCardPos, 1f)
                .OnComplete(() => onComplete?.Invoke());
        }
    }

    protected override bool DragSuccessful() =>
        TableVisual.CursorOverSomeTable && PlayerOwner.Table.CreaturesOnTable.Count < 8;
}