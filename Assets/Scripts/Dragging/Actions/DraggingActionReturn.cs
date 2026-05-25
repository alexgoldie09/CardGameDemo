using System;
using UnityEngine;
using DG.Tweening;

public class DraggingActionReturn : DraggingActions
{
    private Vector3 _savedPos;

    /// <summary>
    /// Saves the initial position of the object when the drag starts, so that it can be returned to this position when the drag ends.
    /// This allows for a dragging behavior where the object will snap back to its original position after being dragged,
    /// regardless of where it was dropped.
    /// </summary>
    public override void OnStartDrag()
    {
        _savedPos = transform.position;
    }

    /// <summary>
    /// Animates the card back to its original position, then invokes onComplete
    /// so hover previews are only re-enabled once the card has fully returned.
    /// </summary>
    public override void OnEndDrag(Action onComplete = null)
    {
        transform.DOMove(_savedPos, 0.6f).SetEase(Ease.OutQuint)
            .OnComplete(() => onComplete?.Invoke());
    }

    public override void OnDraggingInUpdate(){}

    protected override bool DragSuccessful() => true;
}
