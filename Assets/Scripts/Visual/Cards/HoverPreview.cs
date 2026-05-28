using UnityEngine;
using DG.Tweening;

public class HoverPreview : MonoBehaviour
{
    [SerializeField, Tooltip("If assigned, this GameObject will be turned off when the preview is active. If null, no GameObject will be turned off.")]
    private GameObject TurnThisOffWhenPreviewing;
    [SerializeField, Tooltip("Target local position for the preview GameObject when the preview is active.")]
    private Vector3 TargetPosition;
    [SerializeField, Tooltip("Target scale for the preview GameObject when the preview is active.")]
    private float TargetScale;
    [SerializeField, Tooltip("If assigned, this GameObject will be used as the preview. It should be assigned in the inspector.")]
    private GameObject previewGameObject;
    [SerializeField, Tooltip("If true, the preview will be activated in Awake. If false, it will need to be activated manually.")]
    private bool ActivateInAwake = false;

    /// <summary>
    /// Keeps track of the currently active preview. This allows the system to ensure that only one preview is active at a time,
    /// and to properly disable the previous preview when a new one is activated or when previews are turned off globally.
    /// </summary>
    private static HoverPreview currentlyViewing = null;

    /// <summary>
    /// A static property that controls whether previews are allowed globally.
    /// When set to false, it will disable any active preview and prevent new previews from being activated until it is set back to true.
    /// </summary>
    private static bool _previewsAllowed = true;
    
    /// <summary>
    /// Gets or sets whether previews are allowed globally.
    /// Setting this to false will stop any currently active preview and prevent new previews from being activated.
    /// </summary>
    public static bool PreviewsAllowed
    {
        get => _previewsAllowed;
        set
        {
            _previewsAllowed = value;
            if (!_previewsAllowed)
                StopAllPreviews();
        }
    }

    /// <summary>
    /// A property that controls whether this specific preview is enabled.
    /// When set to false, it will stop this preview if it is currently active and prevent it from being activated until it is set back to true.
    /// This allows for individual control over which objects can show previews, in addition to the global control provided by PreviewsAllowed.
    /// </summary>
    private bool _thisPreviewEnabled = false;
    
    /// <summary>
    /// Gets or sets whether this specific preview is enabled.
    /// Setting this to false will stop this preview if it is currently active and prevent it from being activated until it is set back to true.
    /// </summary>
    public bool ThisPreviewEnabled
    {
        get => _thisPreviewEnabled;
        set
        {
            _thisPreviewEnabled = value;
            if (!_thisPreviewEnabled)
                StopThisPreview();
        }
    }

    /// <summary>
    /// Initializes the preview state based on the ActivateInAwake setting.
    /// If ActivateInAwake is true, this preview will be enabled immediately when the object awakens.
    /// </summary>
    private void Awake()
    {
        ThisPreviewEnabled = ActivateInAwake;
    }

    /// <summary>
    /// Subscribes to the CardInputManager's hover events when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        CardInputManager.OnCardHoverEnter += HandleHoverEnter;
        CardInputManager.OnCardHoverExit  += HandleHoverExit;
    }

    /// <summary>
    /// Unsubscribes from the CardInputManager's hover events when this component is disabled to prevent memory leaks and unintended behavior.
    /// </summary>
    private void OnDisable()
    {
        CardInputManager.OnCardHoverEnter -= HandleHoverEnter;
        CardInputManager.OnCardHoverExit  -= HandleHoverExit;
    }

    /// <summary>
    /// Handles the hover enter event from the CardInputManager.
    /// If the hovered card is this object and previews are allowed, it will activate the preview for this object.
    /// </summary>
    /// <param name="card"></param>
    private void HandleHoverEnter(GameObject card)
    {
        if (card != gameObject) return;
        if (PreviewsAllowed && ThisPreviewEnabled)
            PreviewThisObject();
    }

    /// <summary>
    /// Handles the hover exit event from the CardInputManager.
    /// If the exited card is this object, it will stop the preview for this object.
    /// </summary>
    /// <param name="card"></param>
    private void HandleHoverExit(GameObject card)
    {
        if (card != gameObject) return;
        StopAllPreviews();
    }

    /// <summary>
    /// Activates the preview for this object.
    /// It first stops any currently active preview to ensure only one preview is shown at a time,
    /// then it sets this object as the currently viewing preview and activates the preview GameObject.
    /// </summary>
    private void PreviewThisObject()
    {
        // 1. First disable the previous preivew if there is one already
        StopAllPreviews();
        // 2. Save this HoverPreview as current
        currentlyViewing = this;
        // 3. Enable preview game object
        previewGameObject.SetActive(true);
        // 4. Disable if we have what to disable
        if (TurnThisOffWhenPreviewing != null)
            TurnThisOffWhenPreviewing.SetActive(false);
        // 5. Tween to target position and target scale
        previewGameObject.transform.localPosition = Vector3.zero;
        previewGameObject.transform.localScale = Vector3.one;
        Vector3 target = gameObject.tag.Contains("Top")
            ? new Vector3(TargetPosition.x, -TargetPosition.y, TargetPosition.z)
            : TargetPosition;
        previewGameObject.transform.DOLocalMove(target, 1f).SetEase(Ease.OutQuint);
        previewGameObject.transform.DOScale(TargetScale, 1f).SetEase(Ease.OutQuint);
    }

    /// <summary>
    /// Deactivates the preview for this object.
    /// </summary>
    private void StopThisPreview()
    {
        previewGameObject.SetActive(false);
        previewGameObject.transform.localScale = Vector3.one;
        previewGameObject.transform.localPosition = Vector3.zero;

        if (TurnThisOffWhenPreviewing != null)
            TurnThisOffWhenPreviewing.SetActive(true);
    }

    /// <summary>
    /// Deactivates any currently active preview.
    /// </summary>
    private static void StopAllPreviews()
    {
        if (currentlyViewing == null) return;

        currentlyViewing.previewGameObject.SetActive(false);
        currentlyViewing.previewGameObject.transform.localScale = Vector3.one;
        currentlyViewing.previewGameObject.transform.localPosition = Vector3.zero;

        if (currentlyViewing.TurnThisOffWhenPreviewing != null)
            currentlyViewing.TurnThisOffWhenPreviewing.SetActive(true);

        currentlyViewing = null;
    }
}