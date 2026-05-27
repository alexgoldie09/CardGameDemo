using UnityEngine;

public class HeroPowerButton : MonoBehaviour, IClickable
{
    [SerializeField, Tooltip("Which player area this hero power button belongs to.")]
    private AreaPosition owner;

    [Header("Button Visuals")]
    [SerializeField, Tooltip("Front visual shown when hero power is available.")]
    private GameObject front;

    [SerializeField, Tooltip("Back visual shown when hero power has been used this turn.")]
    private GameObject back;

    [SerializeField, Tooltip("Glow visual shown when this button is highlighted.")]
    private GameObject glow;

    #region Accessors
    public AreaPosition Owner => owner;
    public GameObject Front => front;
    public GameObject Back => back;
    public GameObject Glow => glow;

    private bool _wasUsed = false;
    public bool WasUsedThisTurn
    {
        get => _wasUsed;
        set
        {
            _wasUsed = value;
            front.SetActive(!_wasUsed);
            back.SetActive(_wasUsed);
            if (_wasUsed)
                Highlighted = false;
        }
    }

    private bool _highlighted = false;
    public bool Highlighted
    {
        get => _highlighted;
        set
        {
            _highlighted = value;
            glow.SetActive(_highlighted);
        }
    }
    #endregion

    public void OnClick()
    {
        if (!WasUsedThisTurn && Highlighted)
        {
            GlobalSettings.Instance.Players[owner].UseHeroPower();
            WasUsedThisTurn = true;
        }
    }
}