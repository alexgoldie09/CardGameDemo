using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RopeTimer : MonoBehaviour, IEventSystemHandler
{
    [Header("Rope Timer Properties")]
    [SerializeField, Tooltip("The GameObject that contains the rope burn visual.")]
    private GameObject ropeGameObject;
    [SerializeField, Tooltip("The slider that visually represents the rope burning down.")]
    private Slider ropeSlider;
    [SerializeField, Tooltip("The total time in seconds a player has for their turn.")]
    private float timeForOneTurn;
    [SerializeField, Tooltip("The time in seconds before the end of turn that the rope starts burning.")]
    private float ropeBurnTime;
    [SerializeField, Tooltip("The UI text element that displays the countdown timer.")]
    private TextMeshProUGUI timerText;
    [SerializeField, Tooltip("Event invoked when the timer reaches zero.")]
    private UnityEvent timerExpired = new UnityEvent();

    private float timeTillZero;
    private bool counting = false;
    private bool ropeIsBurning;

    #region Accessors
    public GameObject RopeGameObject => ropeGameObject;
    public Slider RopeSlider => ropeSlider;
    public float TimeForOneTurn => timeForOneTurn;
    public float RopeBurnTime => ropeBurnTime;
    public TextMeshProUGUI TimerText => timerText;
    public UnityEvent TimerExpired => timerExpired;
    #endregion

    /// <summary>
    /// Initializes the rope timer by setting the slider's min and max values and hiding the rope visual.
    /// </summary>
    private void Awake()
    {
        if (ropeGameObject != null)
        {
            ropeSlider.minValue = 0;
            ropeSlider.maxValue = ropeBurnTime;
            ropeGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Starts the turn timer by resetting the time till zero, enabling counting, and hiding the rope visual.
    /// </summary>
    public void StartTimer()
    {
        timeTillZero = timeForOneTurn;
        counting = true;
        ropeIsBurning = false;
        if (ropeGameObject != null)
            ropeGameObject.SetActive(false);
    }

    /// <summary>
    /// Stops the turn timer by disabling counting.
    /// The timer will no longer update until StartTimer() is called again.
    /// </summary>
    public void StopTimer()
    {
        counting = false;
    }

    /// <summary>
    /// Updates the timer each frame. If counting is enabled, it decreases the time till zero by the
    /// time elapsed since the last frame. It also updates the timer text and manages the rope burn
    /// visual based on the remaining time. If the timer reaches zero, it invokes the timer expired
    /// event and stops counting.
    /// </summary>
    private void Update()
    {
        if (!counting) return;
        
        timeTillZero -= Time.deltaTime;
        if (timerText != null)
            timerText.text = ToString();

        if (ropeGameObject != null)
        {
            if (timeTillZero <= ropeBurnTime && !ropeIsBurning)
            {
                ropeIsBurning = true;
                ropeGameObject.SetActive(true);
            }
            if (ropeIsBurning)
            {
                ropeSlider.value = timeTillZero;
            }
        }

        if (timeTillZero <= 0)
        {
            counting = false;
            timerExpired?.Invoke();
        }
    }

    public override string ToString()
    {
        int inSeconds = Mathf.RoundToInt(timeTillZero);
        string justSeconds = (inSeconds % 60).ToString();
        if (justSeconds.Length == 1)
            justSeconds = "0" + justSeconds;
        string justMinutes = (inSeconds / 60).ToString();
        if (justMinutes.Length == 1)
            justMinutes = "0" + justMinutes;

        return $"{justMinutes}:{justSeconds}";
    }
}
