using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureInfoManager : MonoBehaviour
{
    [Header("Card Properties")]
    [SerializeField, Tooltip("Card asset that we want to display on this card. It should be assigned in the inspector.")]
    private CardAsset cardAsset;
    [SerializeField, Tooltip("Card image that we want to display on this card.")]
    public CreatureInfoManager PreviewManager;
    [SerializeField, Tooltip("Whether this card is not a preview. " +
                             "This is used to prevent infinite recursion when loading the preview of a card, which also has a preview manager that tries to load the same card again.")]
    public bool IsPreview = false;
    
    [Header("Text Properties")]
    [SerializeField, Tooltip("Health text object on this card.")]
    public TextMeshProUGUI HealthText;
    [SerializeField, Tooltip("Attack text object on this card.")]
    public TextMeshProUGUI AttackText;
    [SerializeField, Tooltip("Name text object on this card.")]
    public TextMeshProUGUI NameText;
    [SerializeField, Tooltip("Description text object on this card.")]
    public TextMeshProUGUI DescriptionText;
    
    [Header("Image Properties")]
    [SerializeField, Tooltip("Card graphic object on this card.")]
    public Image CardGraphicImage;
    [SerializeField, Tooltip("Card top ribbon object on this card.")]
    public Image CardTopRibbonImage;
    [SerializeField, Tooltip("Card bottom ribbon object on this card.")]
    public Image CardBottomRibbonImage;
    [SerializeField, Tooltip("Card body object on this card.")]
    public Image CardBodyImage;
    [SerializeField, Tooltip("Card face frame object on this card.")]
    public Image CardFaceFrameImage;
    [SerializeField, Tooltip("Card glow object on this creature.")]
    public Image CardGlowImage;
    
    private bool canAttackNow = false;

    #region Accessors
    public bool CanAttackNow
    {
        get
        {
            return canAttackNow;
        }

        set
        {
            canAttackNow = value;

            CardGlowImage.enabled = value;
        }
    }
    
    // Expose the asset so DragManager can read it from the dragged card
    public CardAsset CardAsset
    {
        get => cardAsset;
        set => cardAsset = value;
    }

    // Allow the preview panel to be populated from outside
    public void LoadCreature(CardAsset asset)
    {
        cardAsset = asset;
        InitCreature();
    }
    #endregion
    
    // Initialise card info
    private void Awake()
    {
        if (cardAsset != null)
            InitCreature();
    }

    private void InitCreature()
    {
        if (IsPreview)
        {
            if (cardAsset.CharacterAsset != null)
            {
                CardBodyImage.color = cardAsset.CharacterAsset.ClassCardTint;
                CardFaceFrameImage.color = cardAsset.CharacterAsset.ClassCardTint;
                CardTopRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
                CardBottomRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
            }
            else
            {
                CardBodyImage.color = GlobalSettings.Instance.CardBodyStandardColor;
                CardFaceFrameImage.color = Color.white;
                CardTopRibbonImage.color = GlobalSettings.Instance.CardRibbonsStandardColor;
                CardBottomRibbonImage.color = GlobalSettings.Instance.CardRibbonsStandardColor;
            }
            
            NameText.text = cardAsset.name;
            DescriptionText.text = cardAsset.Description;
        }

        // Change the card graphic sprite
        CardGraphicImage.sprite = cardAsset.CardImage;

        if (cardAsset.MaxHealth != 0 && !cardAsset.IsSpell)
        {
            // this is a creature
            AttackText.text = cardAsset.Attack.ToString();
            HealthText.text = cardAsset.MaxHealth.ToString();
        }

        if (PreviewManager != null)
        {
            // this is a card and not a preview
            // Preview GameObject will have CardInfoManager as well, but PreviewManager should be null there
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.InitCreature();
        }
    }
    
    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            DamageEffect.CreateDamageEffect(transform.position, amount);
            HealthText.text = healthAfter.ToString();
            UpdateHealthDisplay(healthAfter);
        }
    }

    public void UpdateHealthDisplay(int healthAfter)
    {
        HealthText.text = healthAfter.ToString();
        if (PreviewManager != null)
            PreviewManager.HealthText.text = healthAfter.ToString();
    }
}
