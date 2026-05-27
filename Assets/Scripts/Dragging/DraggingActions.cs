using System;
using UnityEngine;

public abstract class DraggingActions : MonoBehaviour
{
    /// <summary>
    /// The player who owns this card or creature, determined by the tag of the GameObject.
    /// </summary>
    protected virtual Player PlayerOwner
    {
        get
        {

            if (tag.Contains("Low"))
                return GlobalSettings.Instance.LowPlayer;
            
            if (tag.Contains("Top"))
                return GlobalSettings.Instance.TopPlayer;
            
            Debug.LogError("Untagged Card or creature " + transform.parent.name);
            return null;
        }
    }
    
    /// <summary>
    /// Called once at the start of the drag.
    /// Use this to set up any necessary variables or states for the dragging action.
    /// </summary>
    public abstract void OnStartDrag();

    /// <summary>
    /// Called once at the end of the drag. Invoke onComplete when any exit animation
    /// or cleanup has finished so the input manager knows it is safe to resume hover previews.
    /// </summary>
    public abstract void OnEndDrag(Action onComplete = null);

    /// <summary>
    /// Called every frame while the object is being dragged.
    /// Use this to perform any continuous updates or checks that should happen during the dragging process,
    /// such as updating the position of the dragged object, checking for valid drop targets,
    /// or providing visual feedback to the user.
    /// </summary>
    public abstract void OnDraggingInUpdate();

    /// <summary>
    /// Indicates whether the object can be dragged.
    /// </summary>
    public virtual bool CanDrag => GlobalSettings.Instance.CanControlThisPlayer(PlayerOwner);

    /// <summary>
    /// Determines whether the drag action was successful or not.
    /// </summary>
    /// <returns></returns>
    protected abstract bool DragSuccessful();
}
