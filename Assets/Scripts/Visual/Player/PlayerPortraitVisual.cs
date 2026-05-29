using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class PlayerPortraitVisual : MonoBehaviour 
{

    [SerializeField, Tooltip("The character asset that defines the look of this portrait. " +
                                     "Make sure to assign this in the inspector, otherwise the portrait will not display correctly.")]
    private CharacterAsset charAsset;
    
    [Header("Text Component References")]
    //public Text NameText;
    [FormerlySerializedAs("HealthText")]
    [SerializeField, Tooltip("The text component that will display the health text.")]
    private TextMeshProUGUI healthText;
    [SerializeField, Tooltip("The text component that will display the mana cost text.")]
    private TextMeshProUGUI manaCostText;
    
    [Header("Image References")]
    [SerializeField, Tooltip("The image that will display the hero power icon image.")]
    private Image HeroPowerIconImage;
    [SerializeField, Tooltip("The image that will display the hero power background portrait.")]
    private Image HeroPowerBackgroundImage;
    [SerializeField, Tooltip("The image that will display the hero portrait.")]
    private Image PortraitImage;
    [SerializeField, Tooltip("The image that will display the portrait background portrait.")]
    private Image PortraitBackgroundImage;

    /// <summary>
    /// Initialise the portrait look based on the assigned character asset.
    /// </summary>
    private void Awake()
	{
		if(charAsset != null)
			ApplyLookFromAsset();
	}
	
    /// <summary>
    /// Apply the look of this portrait based on the assigned character asset.
    /// This should be called whenever we want to update the portrait look,
    /// for example when we assign a new character asset to this portrait.
    /// </summary>
	private void ApplyLookFromAsset()
    {
        // Health text update
        healthText.text = charAsset.MaxHealth.ToString();
        manaCostText.text = charAsset.HeroPowerManaCost.ToString();
        
        // Hero power icon image and background image update
        HeroPowerIconImage.sprite = charAsset.HeroPowerIconImage;
        
        if (charAsset.HeroPowerBGImage != null)
            HeroPowerBackgroundImage.sprite = charAsset.HeroPowerBGImage;
        else
            HeroPowerBackgroundImage.color = charAsset.HeroPowerBGTint;

        // Portrait image and background image update
        PortraitImage.sprite = charAsset.AvatarImage;
        if (charAsset.AvatarBGImage != null)
            PortraitBackgroundImage.sprite = charAsset.AvatarBGImage;
        else
            PortraitBackgroundImage.color = charAsset.AvatarBGTint;
    }

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            DamageEffect.CreateDamageEffect(transform.position, amount);
            healthText.text = healthAfter.ToString();
        }
    }

    public void Explode()
    {
        Instantiate(GlobalSettings.Instance.ExplosionPrefab, transform.position, Quaternion.identity);
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => GlobalSettings.Instance.GameOverPanel.SetActive(true));
    }
    
    public TextMeshProUGUI HealthText => healthText;
    public CharacterAsset CharAsset 
    { 
        get => charAsset;
        set
        {
            charAsset = value;
            ApplyLookFromAsset();
        }
    }
}
