using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ManaPoolVisual : MonoBehaviour 
{
    [Header("Debug")]
    [SerializeField, Tooltip("Number of crystals to show in the editor. " +
                             "This is only for testing purposes and will not affect the actual mana pool during gameplay.")]
    private int TestFullCrystals;
    [SerializeField, Tooltip("Number of crystals to show as available in the editor. " +
                             "This is only for testing purposes and will not affect the actual mana pool during gameplay.")]
    private int TestTotalCrystalsThisTurn;
    
    [Header("Crystals Properties")]
    [SerializeField, Tooltip("Number of crystals to show.")]
    private Image[] Crystals;
    [SerializeField, Tooltip("Text object to show the mana progress (e.g. 3/10).")]
    public TextMeshProUGUI ProgressText;

    private int _totalCrystals;
    private int _availableCrystals;

    /// <summary>
    /// Total number of crystals in the mana pool.
    /// This is usually determined by the turn number and can be increased by certain card effects.
    /// It should never be higher than the total number of crystal images assigned to this script.
    /// </summary>
    public int TotalCrystals
    {
        get => _totalCrystals;

        set
        {
            //Debug.Log("Changed total mana to: " + value);

            if (value > Crystals.Length)
                _totalCrystals = Crystals.Length;
            else if (value < 0)
                _totalCrystals = 0;
            else
                _totalCrystals = value;

            for (int i = 0; i < Crystals.Length; i++)
            {
                if (i < _totalCrystals)
                {
                    if (Crystals[i].color == Color.clear)
                        Crystals[i].color = Color.gray;
                }
                else
                    Crystals[i].color = Color.clear;
            }

            // update the text
            ProgressText.text = $"{_availableCrystals.ToString()}/{_totalCrystals.ToString()}";
        }
    }

    /// <summary>
    /// Number of available crystals that the player can currently use.
    /// This is usually equal to TotalCrystals at the start of the turn,
    /// but can be reduced by playing cards or using certain effects.
    /// </summary>
    public int AvailableCrystals
    {
        get => _availableCrystals;

        set
        {
            //Debug.Log("Changed mana this turn to: " + value);

            if (value > _totalCrystals)
                _availableCrystals = _totalCrystals;
            else if (value < 0)
                _availableCrystals = 0;
            else
                _availableCrystals = value;

            for (int i = 0; i < _totalCrystals; i++)
            {
                if (i < _availableCrystals)
                    Crystals[i].color = Color.white;
                else
                    Crystals[i].color = Color.gray;
            }

            // update the text
            ProgressText.text = $"{_availableCrystals.ToString()}/{_totalCrystals.ToString()}";

        }
    }

    private void Update()
    {
        if (!Application.isEditor || Application.isPlaying) return;
        
        if (Crystals == null || Crystals.Length == 0 || ProgressText == null)  return;
        
        TotalCrystals = TestTotalCrystalsThisTurn;
        AvailableCrystals = TestFullCrystals;
    }
	
}
