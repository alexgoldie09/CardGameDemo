using UnityEngine;

public class PlayerDeckVisual : MonoBehaviour
{
    [SerializeField, Tooltip("The player who owns this deck.")]
    private AreaPosition Owner;

    [SerializeField, Tooltip("The height of a single card in the deck.")]
    private float HeightOfOneCard = 0.012f;
    
    private int _cardsInDeck = 0;
    
    #region Accessors and Mutators
    /// <summary>
    /// The number of cards currently in the deck.
    /// Setting this value will adjust the position of the deck visual to reflect the number of cards remaining.
    /// </summary>
    public int CardsInDeck
    {
        get => _cardsInDeck;

        set
        {
            _cardsInDeck = value;
            transform.position = new Vector3(transform.position.x, transform.position.y, - HeightOfOneCard * value);
        }
    }
    #endregion

    private void Start()
    {
        CardsInDeck = GlobalSettings.Instance.Players[Owner].Deck.Cards.Count;
    }
}
