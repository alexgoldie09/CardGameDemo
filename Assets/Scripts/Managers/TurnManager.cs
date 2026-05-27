using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

// this class will take care of switching turns and counting down time until the turn expires
public class TurnManager : MonoBehaviour 
{
    public static TurnManager Instance;
    
    [SerializeField, Tooltip("The card asset for the coin card, which is given to the player who goes second " +
                             "at the start of the game.")]
    private CardAsset coinCard;
    
    private RopeTimer _timer;
    private Player _whoseTurn;
    
    #region Accessors
    public RopeTimer Timer => _timer;
    public CardAsset CoinCard => coinCard;
    public Player WhoseTurn
    {
        get => _whoseTurn;

        set
        {
            _whoseTurn = value;
            _timer.StartTimer();

            GlobalSettings.Instance.EnableEndTurnButtonOnStart(_whoseTurn);

            TurnMaker tm = WhoseTurn.GetComponent<TurnMaker>();
            // player`s method OnTurnStart() will be called in tm.OnTurnStart();
            tm.OnTurnStart();
            if (tm is PlayerTurnMaker)
            {
                WhoseTurn.HighlightPlayableCards();
            }
            // remove highlights for opponent.
            WhoseTurn.OtherPlayer.HighlightPlayableCards(true);
                
        }
    }
    #endregion
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        // Get rope timer component
        _timer = GetComponent<RopeTimer>();
    }

    private void Start() => OnGameStart();
    
    /// <summary>
    /// This method is called at the start of the game.
    /// It will initialize all necessary variables and display the starting animation.
    /// </summary>
    public void OnGameStart()
    {
        //Debug.Log("In TurnManager.OnGameStart()");

        CardLogic.CardsCreatedThisGame.Clear();
        CreatureLogic.CreaturesCreatedThisGame.Clear();

        foreach (var p in Player.Players)
        {
            p.ManaThisTurn = 0;
            p.ManaLeft = 0;
            p.LoadCharacterInfoFromAsset();
            p.TransmitInfoAboutPlayerToVisual();
            p.PArea.PDeck.CardsInDeck = p.Deck.Cards.Count;
            // move both portraits to the center
            p.PArea.Portrait.transform.position = p.PArea.InitialPortraitPosition.position;
        }

        Sequence s = DOTween.Sequence();
        s.Append(Player.Players[0].PArea.Portrait.transform.DOMove(Player.Players[0].PArea.PortraitPosition.position, 1f).SetEase(Ease.InQuad));
        s.Insert(0f, Player.Players[1].PArea.Portrait.transform.DOMove(Player.Players[1].PArea.PortraitPosition.position, 1f).SetEase(Ease.InQuad));
        s.PrependInterval(3f);
        s.OnComplete(() =>
            {
                // determine who starts the game.
                int rnd = Random.Range(0,2);  // 2 is exclusive boundary
                // Debug.Log(Player.Players.Length);
                Player whoGoesFirst = Player.Players[rnd];
                // Debug.Log(whoGoesFirst);
                Player whoGoesSecond = whoGoesFirst.OtherPlayer;
                // Debug.Log(whoGoesSecond);
         
                // draw 4 cards for first player and 5 for second player
                int initDraw = 4;
                for (int i = 0; i < initDraw; i++)
                {            
                    // second player draws a card
                    whoGoesSecond.DrawACard(true);
                    // first player draws a card
                    whoGoesFirst.DrawACard(true);
                }
                // add one more card to second player`s hand
                whoGoesSecond.DrawACard(true);
                //new GivePlayerACoinCommand(null, whoGoesSecond).AddToQueue();
                whoGoesSecond.GetACardNotFromDeck(CoinCard);
                new StartATurnCommand(whoGoesFirst).AddToQueue();
            });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            EndTurn();
    }

    [ContextMenu("End Turn")]
    public void EndTurnTest()
    {
        _timer.StopTimer();
        _timer.StartTimer();
    }

    /// <summary>
    /// This method is called when the current player ends their turn,
    /// either by clicking the end turn button or when the timer runs out.
    /// </summary>
    public void EndTurn()
    {
        // stop timer
        _timer.StopTimer();
        // send all commands in the end of current player`s turn
        WhoseTurn.OnTurnEnd();

        new StartATurnCommand(WhoseTurn.OtherPlayer).AddToQueue();
    }

    /// <summary>
    /// This method is called to stop the timer, for example when the player ends their turn before the timer runs out.
    /// </summary>
    public void StopTheTimer() => _timer.StopTimer();
}

