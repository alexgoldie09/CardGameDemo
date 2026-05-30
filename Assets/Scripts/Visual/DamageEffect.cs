using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// This class will show damage dealt to creatures or players
/// </summary>

public class DamageEffect : MonoBehaviour 
{
    [Header("Damage Effect Properties")]
    [SerializeField, Tooltip("An array of sprites with different blood splash graphics. " +
                                     "A random one will be picked for each damage effect instance to add some visual variety.")]
    private Sprite[] Splashes;

    [SerializeField, Min(0.01f), Tooltip("Seconds to fade out from 1 to 0 alpha.")]
    private float fadeOutDuration = 0.25f;

    [Header("Canvas Properties")]
    [SerializeField, Tooltip("The image component that will display the blood splash graphic. " +
                             "Make sure to assign this in the inspector, otherwise the damage effect will not display correctly.")]
    private Image DamageImage;
    
    [SerializeField, Tooltip("The CanvasGroup component attached to the Canvas of this damage effect. " +
                             "It is used to control the fading of this effect. " +
                                     "Make sure to assign this in the inspector, otherwise the damage effect will not fade correctly.")]
    private CanvasGroup cg;

    [SerializeField, Tooltip("The text component that will display the amount of damage taken. " +
                             "Make sure to assign this in the inspector, otherwise the damage amount will not be displayed.")]
    private TextMeshProUGUI AmountText;
    
    [Header("Debugging Properties")]
    [SerializeField, Tooltip("Whether to add debug logs for this damage effect. " +
                             "This is only for testing purposes and should be turned off in the final version.")]
    private bool AddDebug =  false;
    [SerializeField, Tooltip("The maximum amount of damage that can be displayed by this effect. " +
                             "This is only for testing purposes and does not affect the actual damage calculation in the game.")]
    private int MaxAmountToDamage = 20;

    /// <summary>
    /// Initialise the damage effect by picking a random blood splash image from the assigned array of sprites.
    /// </summary>
    private void Awake()
    {
        // pick a random image
        DamageImage.sprite = Splashes[Random.Range(0, Splashes.Length)];

        // If debug allowed, set the amount to random number
        if (AddDebug)
            AmountText.text = $"-{Random.Range(1, MaxAmountToDamage)}";
    }

    // A Coroutine to control the fading of this damage effect
    private IEnumerator ShowDamageEffect()
    {
        // make this effect non-transparent
        cg.alpha = 1f;
        float t = 0f;
        // wait for 1 second before fading
        yield return new WaitForSeconds(1f);
        // gradually fade the effect by changing its alpha value
        // Fade out (1 -> 0), frame-rate independent
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(1f - (t / fadeOutDuration));
            yield return null;
        }
        // after the effect is shown it gets destroyed.
        Destroy(gameObject);
    }
    
    [ContextMenu("Show Damage Effect")]
    void CallShowDamageEffect() 
    {
        StartCoroutine(ShowDamageEffect());
    }
    
    /// <summary>
    /// Creates the damage effect.
    /// This is a static method, so it should be called like this: DamageEffect.CreateDamageEffect(transform.position, 5);
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="amount">Amount.</param>
    
    public static void CreateDamageEffect(Vector3 position, int amount)
    {
        //Debug.Log($"[DamageEffect] Creating effect at:{position} amount:{amount}");
        if (amount == 0)
            return;
        GameObject newDamageEffect = Instantiate(GlobalSettings.Instance.DamageEffectPrefab, position, Quaternion.identity);
        //Debug.Log($"[DamageEffect] Instantiated: {newDamageEffect != null}");
        // Get DamageEffect component in this new game object
        DamageEffect de = newDamageEffect.GetComponent<DamageEffect>();
        // Change the amount text to reflect the amount of damage dealt
        if (amount < 0)
        {
            // NEGATIVE DAMAGE = HEALING
            de.AmountText.text = "+" + (-amount);
            de.DamageImage.color = Color.green;
        }
        else
            de.AmountText.text = "-"+ amount;
        // start a coroutine to fade away and delete this effect after a certain time
        de.StartCoroutine(de.ShowDamageEffect());
    }
}
