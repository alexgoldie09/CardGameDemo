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
    
    [Header("Text Properties")]
    [SerializeField, Tooltip("Health text object on this card.")]
    public TextMeshProUGUI HealthText;
    [SerializeField, Tooltip("Attack text object on this card.")]
    public TextMeshProUGUI AttackText;
    
    [Header("Image Properties")]
    [SerializeField, Tooltip("Card graphic object on this card.")]
    public Image CardGraphicImage;
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
