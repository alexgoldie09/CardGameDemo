using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField, Tooltip("List of CardAssets that represent the cards in the deck. ")]
    private List<CardAsset> cards = new();

    /// <summary>
    /// Shuffles the deck of cards when the Deck component is initialized.
    /// </summary>
    private void Awake()
    {
        cards.Shuffle();
    }
    
    #region Accessors
    /// <summary>
    /// Returns the list of CardAssets that represent the cards in the deck.
    /// </summary>
    /// <returns></returns>
    public List<CardAsset> Cards => cards;
    #endregion
}
