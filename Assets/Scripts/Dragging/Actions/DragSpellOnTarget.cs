using System;
using UnityEngine;

public class DragSpellOnTarget : DraggingActions
{
    [Header("Targeting")]
    public TargetingOptions targets = TargetingOptions.AllCharacters;

    private SpriteRenderer _spriteRenderer;
    private LineRenderer _lineRenderer;
    private Transform _triangle;
    private SpriteRenderer _triangleRenderer;
    private WhereIsTheCardOrCreature _whereIsThisCard;
    private VisualStates _tempVisualState;
    private CardInfoManager _manager;
    
    public TargetingOptions Targets { get => targets; set => targets = value; }

    public override bool CanDrag => base.CanDrag && _manager.CanBePlayedNow;

    /// <summary>
    /// Initializes all visual components and card references needed for drag targeting.
    /// </summary>
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.sortingLayerName = "AboveEverything";
        _triangle = transform.Find("Triangle");
        _triangleRenderer = _triangle.GetComponent<SpriteRenderer>();
        _manager = GetComponentInParent<CardInfoManager>();
        _whereIsThisCard = GetComponentInParent<WhereIsTheCardOrCreature>();
    }

    /// <summary>
    /// Saves the current visual state and enables arrow visuals when drag starts.
    /// </summary>
    public override void OnStartDrag()
    {
        _tempVisualState = _whereIsThisCard.VisualState;
        _whereIsThisCard.VisualState = VisualStates.Dragging;
        _spriteRenderer.enabled = true;
        _lineRenderer.enabled = true;
    }

    /// <summary>
    /// Raycasts for a valid target on drag end. Plays the spell if a valid target is found,
    /// otherwise restores the card's visual state. Resets arrow visuals regardless.
    /// </summary>
    public override void OnEndDrag(Action onComplete = null)
    {
        GameObject target = FindTarget();
        bool targetValid = false;

        if (target != null)
        {
            Player owner = tag.Contains("Low")
                ? GlobalSettings.Instance.LowPlayer
                : GlobalSettings.Instance.TopPlayer;

            int targetID = target.GetComponent<IDHolder>().UniqueID;
            int cardID = GetComponentInParent<IDHolder>().UniqueID;
            targetValid = TryPlaySpell(owner, cardID, targetID, target);
        }

        if (!targetValid)
        {
            _whereIsThisCard.VisualState = _tempVisualState;
            _whereIsThisCard.SetHandSortingOrder();
        }

        transform.localPosition = new Vector3(0f, 0f, 0.1f);
        _spriteRenderer.enabled  = false;
        _lineRenderer.enabled    = false;
        _triangleRenderer.enabled = false;

        onComplete?.Invoke();
    }

    /// <summary>
    /// Updates the position and rotation of the arrow to point from the card to the drag position.
    /// </summary>
    public override void OnDraggingInUpdate()
    {
        var notNorm = transform.position - transform.parent.position;
        var direction = notNorm.normalized;
        var distanceToTarget = (direction * 2.3f).magnitude;

        if (notNorm.magnitude > distanceToTarget)
        {
            _lineRenderer.SetPositions(new[]
                { transform.parent.position, transform.position - direction * 2.3f });
            _lineRenderer.enabled = true;

            _triangleRenderer.enabled = true;
            _triangleRenderer.transform.position = transform.position - 2f * direction;

            var rotZ = Mathf.Atan2(notNorm.y, notNorm.x) * Mathf.Rad2Deg;
            _triangleRenderer.transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90f);
        }
        else
        {
            _lineRenderer.enabled = false;
            _triangleRenderer.enabled = false;
        }
    }

    protected override bool DragSuccessful() => true;

    #region Helper Methods
    /// <summary>
    /// Raycasts from the camera toward the spell's current position to find a hit target.
    /// Uses RaycastAll since the spell card itself may occlude the target.
    /// </summary>
    private GameObject FindTarget()
    {
        GameObject target = null;
        RaycastHit[] hits = Physics.RaycastAll(
            origin: Camera.main.transform.position,
            direction: (-Camera.main.transform.position + transform.position).normalized,
            maxDistance: 30f
        );

        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag.Contains("Player"))
                target = h.transform.gameObject;
            else if (h.transform.tag.Contains("Creature"))
                target = h.transform.parent.gameObject;
        }

        return target;
    }

    /// <summary>
    /// Validates the target against the spell's targeting options and plays the spell if valid.
    /// Returns true if the spell was played successfully.
    /// </summary>
    private bool TryPlaySpell(Player owner, int cardID, int targetID, GameObject target)
    {
        bool isEnemy    = (tag.Contains("Low") && target.tag.Contains("Top"))
                       || (tag.Contains("Top") && target.tag.Contains("Low"));
        bool isFriendly = (tag.Contains("Low") && target.tag.Contains("Low"))
                       || (tag.Contains("Top") && target.tag.Contains("Top"));
        bool isCreature  = target.tag.Contains("Creature");
        bool isCharacter = isCreature || target.tag.Contains("Player");

        switch (targets)
        {
            case TargetingOptions.AllCharacters:
                owner.PlayASpellFromHand(cardID, targetID);
                return true;

            case TargetingOptions.AllCreatures:
                if (!isCreature) return false;
                owner.PlayASpellFromHand(cardID, targetID);
                return true;

            case TargetingOptions.EnemyCharacters:
                if (!isCharacter || !isEnemy) return false;
                owner.PlayASpellFromHand(cardID, targetID);
                return true;

            case TargetingOptions.EnemyCreatures:
                if (!isCreature || !isEnemy) return false;
                owner.PlayASpellFromHand(cardID, targetID);
                return true;

            case TargetingOptions.YourCharacters:
                if (!isCharacter || !isFriendly) return false;
                owner.PlayASpellFromHand(cardID, targetID);
                return true;

            case TargetingOptions.YourCreatures:
                if (!isCreature || !isFriendly) return false;
                owner.PlayASpellFromHand(cardID, targetID);
                return true;

            default:
                Debug.LogWarning("Reached default case in DraggingActionOnTarget. Suspicious behaviour!");
                return false;
        }
    }
    #endregion
}