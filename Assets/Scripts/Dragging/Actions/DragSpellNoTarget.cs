using System;
using UnityEngine;
using DG.Tweening;

public class DragSpellNoTarget : DraggingActions
{
    private int _savedHandSlot;
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
        _whereIsCard.VisualState = VisualStates.Dragging;
        _whereIsCard.BringToFront();
    }

    public override void OnDraggingInUpdate() { }

    /// <summary>
    /// Plays the spell if dropped over the table, otherwise animates the card back to its hand slot.
    /// </summary>
    public override void OnEndDrag(Action onComplete = null)
    {
        if (DragSuccessful())
        {
            PlayerOwner.PlayASpellFromHand(GetComponent<IDHolder>().UniqueID, -1);
            onComplete?.Invoke();
        }
        else
        {
            _whereIsCard.Slot = _savedHandSlot;
            _whereIsCard.VisualState = tag.Contains("Low")
                ? VisualStates.LowHand
                : VisualStates.TopHand;

            Vector3 oldCardPos = PlayerOwner.PArea.HandVisual.Slots.Children[_savedHandSlot].transform.localPosition;
            transform.DOLocalMove(oldCardPos, 1f)
                .OnComplete(() => onComplete?.Invoke());
        }
    }

    protected override bool DragSuccessful() => TableVisual.CursorOverSomeTable;
}