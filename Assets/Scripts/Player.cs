using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Condition { Idle,DrawingCard,WaitingOtherPlayers,TakingCards,DrawnDepositCard,ReceivingCards,FlippingCards }
/// <summary>
/// Partisipant - cards are compared between them| 
/// BetWatcher - has lower card than betting players|
/// Better - one of two or more players who has same cards|
/// Taker - has biggest card and takes trick|
/// Defeated - run out of cards
/// </summary>
public enum Role { Partisipant,BetWatcher,Better,Defeated }
public class Player : MonoBehaviour
{
    #region Initialization
    public bool Initialized { get; private set; } = false;    
    public void Init(int index, TurnSequencer ts,Main main, float betDelay, float takeDelay, Sprite deckSprite)
    {       
        m = main;
        Index = index;
        TakeDelay = takeDelay;
        BetDelay = betDelay * Index;
        

        Image[] images = gameObject.GetComponentsInChildren<Image>();
        TMP_Text[] texts = gameObject.GetComponentsInChildren<TMP_Text>();

        foreach (var item in texts)
        {
            if (item.gameObject.name == "State")
                stateText = item;
            if (item.gameObject.name == "CardsLeft")
                CardsLeftText = item;
            if (item.gameObject.name == "PlayerNameText")
                playerNameText = item;

        }
        foreach (var item in images)
        {
            if (item.gameObject.name == "DrawPlace")
            {
                DrawPosition = item.transform.position;
                //turn off image so user don`t see red dots.
                item.enabled = false;
            }
            if (item.gameObject.name == "Deck")
            {
                Deck = item.gameObject;
                Deck.GetComponent<Image>().sprite = deckSprite;
            }
        }

        s = ts;
        Initialized = true;             
    }
    public void ResetPlayer()
    {       
        CancelInvoke();
        role = Role.Partisipant;
        Condition = Condition.Idle;
        Cards.Clear();        
        cardsToFlip.Clear();        
        DrawnCard = null;
        Depositing = aboutToTakeCards = false;       
        drawnCards = 0;
        flippedCards = 0;
        receivedCards = 0;
        deployedCards = 0;
        // get random name for non-human player
        if (Index != 0)
            playerNameText.text = Config.GetRandomPlayerName();       
    }

    private int nextPlayerIndex;
    #endregion
    #region Vars & props

    Main m;
    [SerializeField]
    private TMP_Text stateText = null;
    private TMP_Text playerNameText = null;
    private Role role;
    public Role Role 
    {
        //make sure to setting defeated role is only possible inside player class while resetting the player.
        get { return role; }
        set
        {
            if (role != Role.Defeated)
                role = value;
            if (value == Role.Partisipant)
                drawnCards = 0;
        }
    }
    private Condition condition;
    public Condition Condition
    {
        get { return condition; }
        set
        {
            condition = value;

            if (stateText != null&&Config.ShowPlayerState)
                stateText.text = value.ToString();
           
        }
    }
    public bool Depositing { get; set; } = false;
    private bool isBetting = false;

    TurnSequencer s;
    private float turnDelayBase;
    public float TakeDelay { get; set; } = 1f;
    public float BetDelay { get; set; } = 1f;
    public TMP_Text CardsLeftText { get; private set; }
    public Vector3 DrawPosition { get; private set; }
    public GameObject Deck { get; private set; }
    public List<Card> Cards { get; set; } = new List<Card>();
   
    public Card DrawnCard { get; private set; }
    public bool aboutToTakeCards = false;

    private int drawnCards = 0;

    [SerializeField]
    private List<Card> cardsToFlip = new List<Card>();
    
    [SerializeField]
    private int flippedCards = 0;
    [SerializeField]
    private AudioSource audioSource = null;
    public int Index { get; private set; }

    int deployedCards = 0;
    int receivedCards = 0;
    #endregion
    #region Miscellaneous
    
    public int CardsLeft
    {
        get { return Cards.Count; }
    }
    private Vector3 ShiftCardPosition(Vector3 pos, float xStep, float yStep, float multiplier)
    {
        float x, y, z;

        x = xStep * multiplier + pos.x;
        y = yStep * multiplier + pos.y;

        z = pos.z;

        return new Vector3(x, y, z);
    }
    #endregion
    #region Drawing cards
    
    public Card DrawCardFromTop()
    {
        Card c = null;
        if (Cards.Count > 0)
        {
            c = Cards[Cards.Count - 1];
            Cards.RemoveAt(Cards.Count - 1);
            //update Cards left text
            CardsLeftText.text = CardsLeft.ToString();
            cardsInMotion.Add(c);
            DrawnCard = c;
            //move card to the top of other cards.
            DrawnCard.GO.transform.SetAsLastSibling();

            s.CardsOnTable.Add(c);
            
            DrawnCard.Move(ShiftCardPosition(DrawPosition, Config.XStep, Config.YStep, drawnCards),
            Config.cardMovingSpeed, DoneDrawingCard);
            Condition = Condition.DrawingCard;
            //play draw card sound
            audioSource.Play();

        }
        else
        {
            //here player is requested to draw card which he does not have.
            //it can only happen while betting because quantity of player Cards is checked at the end of each turn.
            Role = Role.Defeated;
            s.DoneDrawingCard(this);

        }
        return c;
    }
    
    
    public void DoneDrawingCard(Card c)
    {
        drawnCards++;

        if (drawnCards >= cardsInMotion.Count)
        {
            Condition = Condition.WaitingOtherPlayers;

            //tell main that drawing card completed
            //if player is depositing, then draw betting card from top.
            if (!Depositing)
            {
                cardsToFlip.Add(DrawnCard);
                BeginFlippingCards();

            }
            else
            {                              
                // if player out of Cards in the middle of bet he loses.
                if (CardsLeft == 0)
                {
                    Depositing = false;
                    Role = Role.Defeated;
                    s.DoneDrawingCard(this);
                }
                else
                {
                    Depositing = false;                   
                    DrawCardFromTop();
                }
            }
        }      
    }
    #endregion
    #region Move trick to deck

    private void StartMovingTrickCards()
    {
        //this happens after flipping cards.
        //play draw card sound
        audioSource.Play();
        Condition = Condition.TakingCards;        
        aboutToTakeCards = false;
        //print("StartMoving taken cards");

        for (int i = 0; i < cardsInMotion.Count; i++)
        {            
            cardsInMotion[i].Move(ShiftCardPosition(Deck.transform.position, Config.DeckShift, 0, i),
                Config.cardMovingSpeed, DoneMovingTakenCard);
        }
    }
   [SerializeField]
    private List<Card> cardsInMotion = new List<Card>();
    public void TakeTrick()
    {
        aboutToTakeCards = true;
        cardsInMotion.Clear();
        
        cardsInMotion.AddRange(s.CardsOnTable);
        s.CardsOnTable.Clear();
        for (int i = 0; i < Cards.Count; i++)                    
            //since new drawn cards must be on top, player deck is behind them, so rearrange player deck to make trick cards
            //fly to the bottom of the player deck.
            Cards[i].GO.transform.SetAsLastSibling();
        

        foreach (var item in cardsInMotion)
        {
            if (item.FaceUp)
                cardsToFlip.Add(item);           
        }
        
        //print("begin flipping cards");
        //prepare to flip Cards        
        Invoke("BeginFlippingCards", TakeDelay);
    }    
    private void DoneMovingTakenCard(Card c)
    {
        receivedCards++;                     
        //end turn after taking all Cards
        if (receivedCards >= cardsInMotion.Count)
        {
            //insert range of taken cards to player cards.
            for (int i = 0; i < cardsInMotion.Count; i++)
            {
                c.GO.transform.SetAsFirstSibling();
            }
            //before Cards are taken from table we must prepare space for them and shift Cards that player holds to the right.
            float mult = cardsInMotion.Count - 1;
            for (int i = 0; i < Cards.Count; i++)
                Cards[i].GO.transform.position = ShiftCardPosition(Cards[i].GO.transform.position, Config.DeckShift, 0, mult);

            Cards.InsertRange(0,cardsInMotion);
            UpdateCardsLeftText();
            
            receivedCards = 0;
            cardsInMotion.Clear();
            s.EndTurn(this);            
        }
    }
    #endregion
    #region Betting
    private void Bet()
    {
        if (CardsLeft > 0)
        {
            Depositing = true;
            DrawCardFromTop();
        }
        else
        {
            Role = Role.Defeated;
            s.DoneDrawingCard(this);
        }
    }
    public void BeginBet(float delay)
    {
        Invoke("Bet", delay);
    }
    #endregion
    #region Flipping Cards
    private void BeginFlippingCards()
    {                  
        //initiate flip
        foreach (var card in cardsToFlip)
        {
            card.Flip(Config.CardFlipSpeed, DoneFlippingCard);
        }        
        Condition = Condition.FlippingCards;
    }   
    public void DoneFlippingCard()
    {        
        flippedCards++;
        //here all cards are flipped
        
        if(flippedCards >= cardsToFlip.Count)
        {            
            cardsToFlip.Clear();            
            flippedCards = 0;

            Condition = Condition.WaitingOtherPlayers;

            if (aboutToTakeCards)
                StartMovingTrickCards();
            else
            {
                cardsInMotion.Clear();
                s.DoneDrawingCard(this);
            }
        }
    }
    #endregion
    #region Receiving Cards from deck
    public void StartReceivingCards()
    {       
        CardsLeftText.text = deployedCards.ToString();        
        cardsInMotion.Clear();       
        BeginReceivingNextCard();
        Condition = Condition.ReceivingCards;
    }
    
    private void BeginReceivingNextCard()
    {
        if (deployedCards < Cards.Count)
        {
            Cards[deployedCards].GO.transform.SetAsLastSibling();

            Cards[deployedCards].Move(ShiftCardPosition(Deck.transform.position, Config.DeckShift, 0, deployedCards),
                Config.cardDistributingSpeed,DoneReceivingCardFromDeck);

            cardsInMotion.Add(Cards[deployedCards]);
            deployedCards++;
            Invoke("BeginReceivingNextCard", Config.ReceiveCardDelay);            
        }
        else
        {
            //make stop dealing sound play only once.
            if (Index == 0)
                m.PlayDealingFinishSound();
            deployedCards = 0;            
        }
    }
    public void DoneReceivingCardFromDeck(Card c)
    {
        receivedCards++;
        CardsLeftText.text = receivedCards.ToString();        
        cardsInMotion.Remove(c);
        //check if this card is last
        if (cardsInMotion.Count == 0)
        {
            deployedCards = 0;
            receivedCards = 0;
            Condition = Condition.Idle;
        }
    }

    #endregion
    #region Testing
    /// <summary>
    /// For testing purposes
    /// </summary>
    public void MakeAllCardsHighestRank()
    {
        foreach (var item in Cards)
        {
            item.Rank = Config.HighestRank;
        }
    }
    public void MakeAllCardsLowestRank()
    {
        foreach (var item in Cards)
        {
            item.Rank = Config.LowestRank;
        }
    }
    #endregion
    #region Update
    public void UpdateCardsLeftText()
    {
        CardsLeftText.text = CardsLeft.ToString();
    }
    private void Update()
    {
        if(Condition == Condition.DrawingCard|| Condition == Condition.TakingCards|| Condition == Condition.ReceivingCards)
            for (int i = 0; i < cardsInMotion.Count; i++)            
                cardsInMotion[i].UpdateMove();
        
        if(Condition== Condition.FlippingCards)
            for (int i = 0; i < cardsToFlip.Count; i++)
                cardsToFlip[i].UpdateFlip();
    }
    #endregion
}
