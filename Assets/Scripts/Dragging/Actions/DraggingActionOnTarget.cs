using System;
using UnityEngine;

public class DraggingActionOnTarget : DraggingActions
{
    // Sprite for main target
    private SpriteRenderer _spriteRenderer;
    // Line renderer for arrow
    private LineRenderer _lineRenderer;
    // Transform for point
    private Transform _triangle;
    // Sprite for point
    private SpriteRenderer _triangleRenderer;

    /// <summary>
    /// Initializes the SpriteRenderer, LineRenderer, and triangle components
    /// used for visual feedback during dragging.
    /// </summary>
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _triangle = transform.Find("Triangle");
        _triangleRenderer = _triangle.GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    /// Enables the SpriteRenderer and LineRenderer components to
    /// provide visual feedback when the drag starts.
    /// </summary>
    public override void OnStartDrag()
    {
        _spriteRenderer.enabled  = true;
        _lineRenderer.enabled = true;
    }

    public override void OnEndDrag(Action onComplete = null)
    {
        // return target and arrow to original pos
        // this position is special for spell cards to show arrow on top
        transform.localPosition = new Vector3(0f, 0f, 0.1f);
        _spriteRenderer.enabled  = false;
        _lineRenderer.enabled = false;
        _triangleRenderer.enabled = false;
        onComplete?.Invoke();
    }

    /// <summary>
    /// Updates the position and rotation of the arrow components to point from the creature to the current drag position.
    /// </summary>
    public override void OnDraggingInUpdate()
    {
        // Draws the arrow
        var notNorm = transform.position - transform.parent.position;
        var direction = notNorm.normalized;
        var distanceToTarget = (direction * 2.3f).magnitude;
        if (notNorm.magnitude > distanceToTarget)
        {
            // Draw a line between the creature and target
            _lineRenderer.SetPositions(new []
                { transform.parent.position, transform.position - direction * 2.3f });
            _lineRenderer.enabled = true;
            
            // Position the end of the arrow between near the target
            _triangleRenderer.enabled = true;
            _triangleRenderer.transform.position = transform.position - 2f * direction;
            
            // Proper rotation of arrow end
            var rotZ = Mathf.Atan2(notNorm.y, notNorm.x) * Mathf.Rad2Deg;
            _triangleRenderer.transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90f);
        }
        else
        {
            // If the target is not far enough from creature, do not show the arrow
            _lineRenderer.enabled = false;
            _triangleRenderer.enabled = false;
        }
    }

    protected override bool DragSuccessful()
    {
        return true;
    }
}
