using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;
    
    [Header("Message Panel References")]
    [SerializeField, Tooltip("The panel that will display the message. It should be assigned in the inspector.")]
    private GameObject MessagePanel;
    [SerializeField, Tooltip("The text component that will display the message text.")]
    private TextMeshProUGUI MessageText;
    
    [Header("Fade Settings")]
    [SerializeField, Tooltip("The canvas group component of the message panel. This is used to control the visibility of the panel.")]
    private CanvasGroup cg;

    [SerializeField, Min(0.01f), Tooltip("Seconds to fade in from 0 to 1 alpha.")]
    private float fadeInDuration = 0.25f;

    [SerializeField, Min(0.01f), Tooltip("Seconds to fade out from 1 to 0 alpha.")]
    private float fadeOutDuration = 0.25f;

    /// <summary>
    /// A reference to the currently active Coroutine that is showing a message.
    /// This is used to stop the Coroutine if a new message needs to be shown before the previous one has finished.
    /// </summary>
    private Coroutine _activeShowMessageCoroutine;

    /// <summary>
    /// Initialise the singleton instance and hide the message panel at the start of the game.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        MessagePanel.SetActive(false);
    }

    /// <summary>
    /// Show a message on the message panel for a specified duration.
    /// After the duration, the message panel will be hidden again.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="duration"></param>
    public void ShowMessage(string message, float duration)
    {
        // Ensure only one message coroutine runs at a time.
        if (_activeShowMessageCoroutine != null)
        {
            StopCoroutine(_activeShowMessageCoroutine);
            _activeShowMessageCoroutine = null;
        }

        _activeShowMessageCoroutine = StartCoroutine(
            ShowMessageCoroutine(message, duration)
        );
    }

    /// <summary>
    /// A Coroutine that controls the display of the message panel.
    /// It will show the message for the specified duration and then hide the panel again.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        MessageText.text = message;
        MessagePanel.SetActive(true);

        // Fade in
        cg.alpha = 0f;
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(t / fadeInDuration);
            yield return null;
        }

        // Hold
        cg.alpha = 1f;
        yield return new WaitForSeconds(duration);

        // Fade out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(1f - (t / fadeOutDuration));
            yield return null;
        }

        cg.alpha = 0f;
        MessagePanel.SetActive(false);
        
        _activeShowMessageCoroutine = null;
        // TODO Command.CommandExecutionComplete();
    }


    #region Debug Messages
    [ContextMenu("Show Message - Your Turn")]
    public void ShowMessageYourTurn()
    {
        ShowMessage("Your Turn", 3f);
    }

    [ContextMenu("Show Message - Enemy Turn")]
    public void ShowMessageEnemyTurn()
    {
        ShowMessage("Enemy Turn", 3f);
    }
    #endregion
}
