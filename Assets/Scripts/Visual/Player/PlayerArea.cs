using UnityEngine;

public enum AreaPosition
{
    Top, 
    Low
}

public class PlayerArea : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Determines if this player area is at the top or bottom of the board")]
    private AreaPosition owner;
    
    [SerializeField]
    [Tooltip("Whether player input controls are currently enabled")]
    private bool controlsOn = true;
    
    [SerializeField]
    [Tooltip("Reference to the player's deck visual representation")]
    private PlayerDeckVisual pDeck;
    
    [SerializeField]
    [Tooltip("Reference to the mana pool visual bar")]
    private ManaPoolVisual manaBar;
    
    [SerializeField]
    [Tooltip("Reference to the player's hand visual")]
    private HandVisual handVisual;
    
    [SerializeField]
    [Tooltip("Reference to the player's portrait visual")]
    private PlayerPortraitVisual portrait;
    
    [SerializeField]
    [Tooltip("Reference to the hero power button")]
    private HeroPowerButton heroPower;
    
    [SerializeField]
    [Tooltip("Reference to the table visual")]
    private TableVisual tableVisual;
    
    [SerializeField]
    [Tooltip("Transform for the portrait position")]
    private Transform portraitPosition;
    
    [SerializeField]
    [Tooltip("Initial position of the portrait")]
    private Transform initialPortraitPosition;

    #region Accessors
    public AreaPosition Owner => owner;
    public bool ControlsOn { get => controlsOn; set => controlsOn = value; }
    public PlayerDeckVisual PDeck => pDeck;
    public ManaPoolVisual ManaBar => manaBar;
    public HandVisual HandVisual => handVisual;
    public PlayerPortraitVisual Portrait => portrait;
    public HeroPowerButton HeroPower => heroPower;
    public TableVisual TableVisual => tableVisual;
    public Transform PortraitPosition => portraitPosition;
    public Transform InitialPortraitPosition => initialPortraitPosition;
    
    /// <summary>
    /// Indicates whether the player associated with this PlayerArea is currently allowed to be controlled by player input.
    /// </summary>
    public bool AllowedToControlThisPlayer
    {
        get;
        set;
    }  
    #endregion
    
}