using System;
using UnityEngine;

public class DragCreatureAttack : DraggingActions
{
    private SpriteRenderer _spriteRenderer;
    private LineRenderer _lineRenderer;
    private WhereIsTheCardOrCreature _whereIsThisCreature;
    private Transform _triangle;
    private SpriteRenderer _triangleRenderer;
    private CreatureInfoManager _manager;

    public override bool CanDrag => base.CanDrag && _manager.CanAttackNow;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.sortingLayerName = "AboveEverything";
        _triangle = transform.Find("Triangle");
        _triangleRenderer = _triangle.GetComponent<SpriteRenderer>();
        _manager = GetComponentInParent<CreatureInfoManager>();
        _whereIsThisCreature = GetComponentInParent<WhereIsTheCardOrCreature>();
    }

    public override void OnStartDrag()
    {
        _whereIsThisCreature.VisualState = VisualStates.Dragging;
        _spriteRenderer.enabled = true;
        _lineRenderer.enabled = true;
    }

    /// <summary>
    /// Raycasts for a valid attack target on drag end.
    /// Attacks the target if valid, otherwise restores the creature's visual state.
    /// </summary>
    public override void OnEndDrag(Action onComplete = null)
    {
        GameObject target = FindTarget();
        bool targetValid = false;

        if (target != null)
            targetValid = TryAttack(target);

        if (!targetValid)
        {
            _whereIsThisCreature.VisualState = tag.Contains("Low")
                ? VisualStates.LowTable
                : VisualStates.TopTable;
            _whereIsThisCreature.SetTableSortingOrder();
        }

        transform.localPosition = Vector3.zero;
        _spriteRenderer.enabled  = false;
        _lineRenderer.enabled    = false;
        _triangleRenderer.enabled = false;

        onComplete?.Invoke();
    }

    /// <summary>
    /// Updates the arrow to point from the creature to the current drag position.
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
    /// Raycasts from the camera toward the creature's position to find an attack target.
    /// Only returns enemy players or enemy creatures as valid hits.
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
            if ((h.transform.CompareTag("TopPlayer") && CompareTag("LowCreature")) ||
                (h.transform.CompareTag("LowPlayer") && CompareTag("TopCreature")))
                target = h.transform.gameObject;
            else if ((h.transform.CompareTag("TopCreature") && CompareTag("LowCreature")) ||
                     (h.transform.CompareTag("LowCreature") && CompareTag("TopCreature")))
                target = h.transform.parent.gameObject;
        }

        return target;
    }

    /// <summary>
    /// Resolves the attack against the target. Returns true if the attack was valid and executed.
    /// </summary>
    private bool TryAttack(GameObject target)
    {
        int targetID  = target.GetComponent<IDHolder>().UniqueID;
        int attackerID = GetComponentInParent<IDHolder>().UniqueID;

        if (targetID == GlobalSettings.Instance.LowPlayer.ID ||
            targetID == GlobalSettings.Instance.TopPlayer.ID)
        {
            Player targetPlayer = targetID == GlobalSettings.Instance.LowPlayer.ID
                ? GlobalSettings.Instance.LowPlayer
                : GlobalSettings.Instance.TopPlayer;
            if (targetPlayer.Table.CreaturesOnTable.Count > 0)
                return false;
            CreatureLogic.CreaturesCreatedThisGame[attackerID].GoFace();
            return true;
        }

        if (CreatureLogic.CreaturesCreatedThisGame.ContainsKey(targetID))
        {
            CreatureLogic.CreaturesCreatedThisGame[attackerID].AttackCreatureWithID(targetID);
            return true;
        }

        return false;
    }
    #endregion
}