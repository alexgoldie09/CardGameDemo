using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoManager : MonoBehaviour
{
    [Header("Card Properties")]
    [SerializeField, Tooltip("Card asset that we want to display on this card. It should be assigned in the inspector.")]
    private CardAsset cardAsset;
    [SerializeField, Tooltip("Card image that we want to display on this card.")]
    public CardInfoManager PreviewManager;
    
    [Header("Text Properties")]
    [SerializeField, Tooltip("Name text object on this card.")]
    public TextMeshProUGUI NameText;
    [SerializeField, Tooltip("Mana cost text object on this card.")]
    public TextMeshProUGUI ManaCostText;
    [SerializeField, Tooltip("Description text object on this card.")]
    public TextMeshProUGUI DescriptionText;
    [SerializeField, Tooltip("Health text object on this card.")]
    public TextMeshProUGUI HealthText;
    [SerializeField, Tooltip("Attack text object on this card.")]
    public TextMeshProUGUI AttackText;
    [SerializeField, Tooltip("Type text object on this card.")]
    public TextMeshProUGUI TypeText;
    
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
    [SerializeField, Tooltip("Card face glow object on this card.")]
    public Image CardFaceGlowImage;
    [SerializeField, Tooltip("Card back glow object on this card.")]
    public Image CardBackGlowImage;
    
    // Is this card being played now
    private bool canBePlayedNow = false;

    #region Accessors
    public bool CanBePlayedNow
    {
        get => canBePlayedNow;
        set
        {
            canBePlayedNow = value;
            CardFaceGlowImage.enabled = value;
        }
    }
    
    // Expose the asset so DragManager can read it from the dragged card
    public CardAsset CardAsset => cardAsset;

    // Allow the preview panel to be populated from outside
    public void LoadCard(CardAsset asset)
    {
        cardAsset = asset;
        InitCard();
    }
    #endregion
    
    // Initialise card info
    private void Awake()
    {
        if (cardAsset != null)
            InitCard();
    }

    private void InitCard()
    {
        // universal actions for any Card
        // 1) apply tint
        if (cardAsset.CharacterAsset != null)
        {
            CardBodyImage.color = cardAsset.CharacterAsset.ClassCardTint;
            CardFaceFrameImage.color = cardAsset.CharacterAsset.ClassCardTint;
            CardTopRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
            CardBottomRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
        }
        else
        {
            //CardBodyImage.color = GlobalSettings.Instance.CardBodyStandardColor;
            CardFaceFrameImage.color = Color.white;
            //CardTopRibbonImage.color = GlobalSettings.Instance.CardRibbonsStandardColor;
            //CardLowRibbonImage.color = GlobalSettings.Instance.CardRibbonsStandardColor;
        }
        // 2) add card name
        NameText.text = cardAsset.name;
        // 3) add mana cost
        ManaCostText.text = cardAsset.ManaCost.ToString();
        // 4) add description
        DescriptionText.text = cardAsset.Description;
        // 5) Change the card graphic sprite
        CardGraphicImage.sprite = cardAsset.CardImage;

        if (cardAsset.MaxHealth != 0 && !cardAsset.IsSpell)
        {
            // this is a creature
            AttackText.text = cardAsset.Attack.ToString();
            HealthText.text = cardAsset.MaxHealth.ToString();
            TypeText.text = "Creature";
        }
        else
        {
            // this is a spell
            TypeText.text = "Spell";
        }

        if (PreviewManager != null)
        {
            // this is a card and not a preview
            // Preview GameObject will have CardInfoManager as well, but PreviewManager should be null there
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.InitCard();
        }
    }
}
