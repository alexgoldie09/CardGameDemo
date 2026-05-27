using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardInputManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("The physics layer that is used to check against for dragging.")]
    private LayerMask draggableLayer;
    [SerializeField, Tooltip("The physics layer used to detect table colliders.")]
    private LayerMask tableLayer;
    [SerializeField, Tooltip("The physics layer used to detect clickable objects such as the hero power button.")]
    private LayerMask clickableLayer;
    [SerializeField, Tooltip("If true, the dragged object will maintain its initial displacement " +
                             "from the pointer. If false, the dragged object will snap to the pointer position.")]
    private bool usePointerDisplacement = true;

    [SerializeField, Tooltip("Reference to a Press action from an InputActionAsset. " +
                             "If unassigned, defaults to left mouse button and gamepad right trigger.")]
    private InputActionReference pressActionReference;
    [SerializeField, Tooltip("Reference to a Point action from an InputActionAsset. " +
                             "If unassigned, defaults to mouse position.")]
    private InputActionReference pointActionReference;

    [Header("Card Preview")]
    [SerializeField, Tooltip("Reference to the card info manager for the creature preview panel.")]
    private CardInfoManager creaturePreviewPanel;
    [SerializeField, Tooltip("Reference to the card info manager for the spell preview panel.")]
    private CardInfoManager spellPreviewPanel;

    // Fired when the pointer enters a card on the draggable layer
    public static event Action<GameObject> OnCardHoverEnter;
    // Fired when the pointer exits a card on the draggable layer
    public static event Action<GameObject> OnCardHoverExit;
    // Fired when a drag starts on a card
    public static event Action<GameObject> OnCardDragStart;
    // Fired when a drag ends on a card
    public static event Action<GameObject> OnCardDragEnd;

    private InputAction pressAction;
    private InputAction pointAction;

    private GameObject currentDragTarget;
    private DraggingActions currentDragActions;
    private GameObject currentHoverTarget;
    private TableVisual[] tables;

    /// <summary>
    /// The displacement between the pointer and the dragged object when the drag starts.
    /// Allows smooth dragging where the object maintains its relative position to the pointer.
    /// </summary>
    private Vector3 pointerDisplacement;

    /// <summary>
    /// The z-axis displacement from the camera to the dragged object.
    /// Calculated on drag start to maintain correct depth while dragging.
    /// </summary>
    private float zDisplacement;

    /// <summary>
    /// Initializes input actions from references if assigned, otherwise falls back to defaults.
    /// </summary>
    private void Awake()
    {
        if (pressActionReference != null)
        {
            pressAction = pressActionReference.action;
        }
        else
        {
            pressAction = new InputAction("Press", InputActionType.Button);
            pressAction.AddBinding("<Mouse>/leftButton");
            pressAction.AddBinding("<Gamepad>/rightTrigger");
        }

        if (pointActionReference != null)
        {
            pointAction = pointActionReference.action;
        }
        else
        {
            pointAction = new InputAction("Point", expectedControlType: "Vector2");
            pointAction.AddBinding("<Mouse>/position");
        }
    }
    
    private void Start()
    {
        tables = FindObjectsByType<TableVisual>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        pressAction.started  += OnPressStarted;
        pressAction.canceled += OnPressCanceled;
        if (pressActionReference == null) pressAction.Enable();
        if (pointActionReference == null) pointAction.Enable();
    }

    private void OnDisable()
    {
        pressAction.started  -= OnPressStarted;
        pressAction.canceled -= OnPressCanceled;
        if (pressActionReference == null) pressAction.Disable();
        if (pointActionReference == null) pointAction.Disable();
    }

    /// <summary>
    /// Handles drag start. Raycasts for a draggable card, sets up drag state,
    /// shows the preview panel, and fires drag and hover events.
    /// </summary>
    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        
        // Check clickables first — if we hit one, handle it and stop
        if (Physics.Raycast(ray, out RaycastHit clickHit, Mathf.Infinity, clickableLayer))
        {
            clickHit.collider.GetComponent<IClickable>()?.OnClick();
            return;
        }

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, draggableLayer)) return;

        var actions = hit.collider.GetComponent<DraggingActions>();
        if (actions != null && !actions.CanDrag) return;

        currentDragTarget  = hit.collider.gameObject;
        currentDragActions = actions;

        zDisplacement = -Camera.main.transform.position.z + currentDragTarget.transform.position.z;
        pointerDisplacement = usePointerDisplacement
            ? -currentDragTarget.transform.position + ScreenToWorld(screenPos)
            : Vector3.zero;

        // Disable hover previews while dragging
        HoverPreview.PreviewsAllowed = false;

        // Clear hover state since we're now dragging
        if (currentHoverTarget != null)
        {
            OnCardHoverExit?.Invoke(currentHoverTarget);
            currentHoverTarget = null;
        }

        var cardInfo = currentDragTarget.GetComponent<CardInfoManager>();
        if (cardInfo != null)
        {
            var panel = ActivePreviewPanel(cardInfo.CardAsset);
            panel.LoadCard(cardInfo.CardAsset);
            panel.CanBePlayedNow = false;
            panel.gameObject.SetActive(true);
        }

        OnCardDragStart?.Invoke(currentDragTarget);
        currentDragActions?.OnStartDrag();
    }

    /// <summary>
    /// Handles drag end. Clears drag state, hides preview panels, and fires drag end event.
    /// </summary>
    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        if (currentDragTarget == null) return;

        creaturePreviewPanel.gameObject.SetActive(false);
        spellPreviewPanel.gameObject.SetActive(false);

        OnCardDragEnd?.Invoke(currentDragTarget);

        if (currentDragActions != null)
        {
            currentDragActions.OnEndDrag(() =>
            {
                ResetTableHover();
                HoverPreview.PreviewsAllowed = true;
            });
        }
        else
        {
            ResetTableHover();
            HoverPreview.PreviewsAllowed = true;
        }

        currentDragTarget  = null;
        currentDragActions = null;
    }

    private void Update()
    {
        if (currentDragTarget != null)
        {
            Vector3 worldPos = ScreenToWorld(pointAction.ReadValue<Vector2>());
            currentDragTarget.transform.position = new Vector3(
                worldPos.x - pointerDisplacement.x,
                worldPos.y - pointerDisplacement.y,
                currentDragTarget.transform.position.z
            );
            currentDragActions?.OnDraggingInUpdate();
            CheckTableHover();
        }
        else
        {
            CheckHover();
        }
    }

    #region Hover Methods
    /// <summary>
    /// Raycasts each frame to detect hover enter and exit on draggable cards.
    /// Fires OnCardHoverEnter and OnCardHoverExit events when the hovered card changes.
    /// </summary>
    private void CheckHover()
    {
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        GameObject newHoverTarget = null;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, draggableLayer))
            newHoverTarget = hit.collider.gameObject;

        if (newHoverTarget == currentHoverTarget) return;

        if (currentHoverTarget != null)
            OnCardHoverExit?.Invoke(currentHoverTarget);

        currentHoverTarget = newHoverTarget;

        if (currentHoverTarget != null)
            OnCardHoverEnter?.Invoke(currentHoverTarget);
    }
    
    /// <summary>
    /// Uses RaycastAll to detect which table the pointer is over during a drag,
    /// since a dragged card may occlude the table collider.
    /// Sets CursorOverThisTable on each TableVisual accordingly.
    /// </summary>
    private void CheckTableHover()
    {
        ResetTableHover();

        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        RaycastHit[] hits = Physics.RaycastAll(
            Camera.main.ScreenPointToRay(screenPos),
            Mathf.Infinity,
            tableLayer
        );

        foreach (RaycastHit hit in hits)
        {
            TableVisual table = hit.collider.GetComponent<TableVisual>();
            if (table != null)
                table.CursorOverThisTable = true;
        }
    }

    /// <summary>
    /// Clears the cursor over state on all cached tables.
    /// </summary>
    private void ResetTableHover()
    {
        foreach (TableVisual table in tables)
            table.CursorOverThisTable = false;
    }
    #endregion

    #region Helper Methods
    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 pos = new Vector3(screenPos.x, screenPos.y, zDisplacement);
        return Camera.main.ScreenToWorldPoint(pos);
    }

    private CardInfoManager ActivePreviewPanel(CardAsset asset) =>
        asset.IsSpell ? spellPreviewPanel : creaturePreviewPanel;
    #endregion
}