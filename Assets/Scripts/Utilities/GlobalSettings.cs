using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance;

    [Header("Players")]
    [SerializeField, Tooltip("The player that is on the top of the screen")]
    private Player topPlayer;
    [SerializeField, Tooltip("The player that is on the bottom of the screen")]
    private Player lowPlayer;

    [Header("Colors")]
    [SerializeField, Tooltip("The color of the card body when it is in its standard state")]
    private Color cardBodyStandardColor;
    [SerializeField, Tooltip("The color of the card ribbons when it is in its standard state")]
    private Color cardRibbonsStandardColor;
    [SerializeField, Tooltip("The color of the card glow when it is in its standard state")]
    private Color cardGlowColor;

    [Header("Numbers and Values")]
    [SerializeField, Tooltip("The time it takes for a card to transition to its preview state when hovered over")]
    private float cardPreviewTime = 1f;
    [SerializeField, Tooltip("The time it takes for a card to transition back to its normal state when no longer hovered over")]
    private float cardTransitionTime = 1f;
    [SerializeField, Tooltip("The time it takes for a card to transition to its preview state when hovered over, for fast transitions (e.g. when the player is holding down a key)")]
    private float cardPreviewTimeFast = 0.2f;
    [SerializeField, Tooltip("The time it takes for a card to transition back to its normal state when no longer hovered over, for fast transitions (e.g. when the player is holding down a key)")]
    private float cardTransitionTimeFast = 0.5f;

    [Header("Prefabs and Assets")]
    [SerializeField, Tooltip("The prefab that is used to place the no target spell card")]
    private GameObject noTargetSpellCardPrefab;
    [SerializeField, Tooltip("The prefab that is used to place the target spell card")]
    private GameObject targetedSpellCardPrefab;
    [SerializeField, Tooltip("The prefab that is used to place the creature")]
    private GameObject creaturePrefab;
    [SerializeField, Tooltip("The prefab that is used to place the creature card")]
    private GameObject creatureCardPrefab;
    [SerializeField, Tooltip("The prefab that is used to place the damage effect")]
    private GameObject damageEffectPrefab;
    [SerializeField, Tooltip("The prefab that is used to place the explosion effect")]
    private GameObject explosionPrefab;

    [Header("Other")]
    [SerializeField, Tooltip("The button that the player clicks to end their turn")]
    private Button endTurnButton;
    [SerializeField, Tooltip("The panel that is displayed when the game is over")]
    private GameObject gameOverPanel;

    #region Accessors
    public Player TopPlayer => topPlayer;
    public Player LowPlayer => lowPlayer;
    public Color CardBodyStandardColor => cardBodyStandardColor;
    public Color CardRibbonsStandardColor => cardRibbonsStandardColor;
    public Color CardGlowColor => cardGlowColor;
    public float CardPreviewTime => cardPreviewTime;
    public float CardTransitionTime => cardTransitionTime;
    public float CardPreviewTimeFast => cardPreviewTimeFast;
    public float CardTransitionTimeFast => cardTransitionTimeFast;
    public GameObject NoTargetSpellCardPrefab => noTargetSpellCardPrefab;
    public GameObject TargetedSpellCardPrefab => targetedSpellCardPrefab;
    public GameObject CreaturePrefab => creaturePrefab;
    public GameObject CreatureCardPrefab => creatureCardPrefab;
    public GameObject DamageEffectPrefab => damageEffectPrefab;
    public GameObject ExplosionPrefab => explosionPrefab;
    public Button EndTurnButton => endTurnButton;
    public GameObject GameOverPanel => gameOverPanel;
    #endregion

    /// <summary>
    /// A dictionary that maps the area positions (top and low) to their respective player objects.
    /// </summary>
    public Dictionary<AreaPosition, Player> Players = new Dictionary<AreaPosition, Player>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Add the players to the dictionary
        Players.Add(AreaPosition.Top, topPlayer);
        Players.Add(AreaPosition.Low, lowPlayer);
    }

    /// <summary>
    /// Returns true if the player in the specified area can be controlled by the local player.
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public bool CanControlThisPlayer(AreaPosition owner)
    {
        bool PlayersTurn = (TurnManager.Instance.WhoseTurn == Players[owner]);
        bool NotDrawingAnyCards = !Command.CardDrawPending();
        return Players[owner].PArea.AllowedToControlThisPlayer && Players[owner].PArea.ControlsOn && PlayersTurn && NotDrawingAnyCards;
    }
    
    /// <summary>
    /// Returns true if the specified player can be controlled by the local player.
    /// </summary>
    /// <param name="ownerPlayer"></param>
    /// <returns></returns>
    public bool CanControlThisPlayer(Player ownerPlayer)
    {
        bool PlayersTurn = (TurnManager.Instance.WhoseTurn == ownerPlayer);
        bool NotDrawingAnyCards = !Command.CardDrawPending();
        return ownerPlayer.PArea.AllowedToControlThisPlayer && ownerPlayer.PArea.ControlsOn && PlayersTurn && NotDrawingAnyCards;
    }
    
    /// <summary>
    /// Enables the end turn button if the specified player can be controlled by the local player, and disables it otherwise.
    /// </summary>
    /// <param name="P"></param>
    public void EnableEndTurnButtonOnStart(Player P)
    {
        if (P == LowPlayer && CanControlThisPlayer(AreaPosition.Low) ||
            P == TopPlayer && CanControlThisPlayer(AreaPosition.Top))
            EndTurnButton.interactable = true;
        else
            EndTurnButton.interactable = false;
            
    }
}
