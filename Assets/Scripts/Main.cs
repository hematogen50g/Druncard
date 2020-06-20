using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Main : MonoBehaviour
{        
    #region Vars
    [SerializeField]
    private GameObject gameEndBackground = null, gameEndStatusPanel = null;
    [SerializeField]
    private TMP_Text gameEndText = null;   
    [SerializeField]
    private Canvas mainMenuCanvas = null;      
    private Image tableImage = null;
    [SerializeField]
    private GameObject table = null;      
     
    /// <summary>
    /// Cards that livePlayers are currently playing.
    /// </summary>
             
    /// <summary>
    /// Text that shows how many cards a player has now.
    /// </summary>
  
    [SerializeField]
    public List<Player> players = new List<Player>();
    [SerializeField]
    public List<Player> ActivePlayers { get; set; } = new List<Player>();
    public int BettingPlayers { get; set; } = 0;
    [SerializeField]
    Transform cardParent = null;       
    private Options options;
    private TurnSequencer turnSequencer;
    private Deck deck;
    private ADS ads;
    private Player taker;
    #endregion    
    #region Sound
    public void PlayButtonSound()
    {
        PlaySound(buttonClick);
    }
    public void PlaySound(AudioClip clip)
    {
        mainSource.clip = clip;
        mainSource.Play();
    }
    public void PlayDealingSound()
    {
        dealingSoundSource.clip = dealCardsProgress;
        dealingSoundSource.loop = true;
        dealingSoundSource.Play();
    }
    public void PlayDealingFinishSound()
    {
        dealingSoundSource.loop = false;
        dealingSoundSource.Stop();       
        dealingSoundSource.clip = dealCardsFinish;
        dealingSoundSource.Play();
    }
   
    public AudioClip buttonClick = null, bet = null, victory = null, defeat = null, dealCardsProgress = null, dealCardsFinish = null;
    
    [SerializeField]
    AudioSource mainSource = null;
    AudioSource dealingSoundSource = null;
    #endregion
    #region Initialization
    public void SetTableSprite(Sprite s)
    {
        tableImage.sprite = s;
    }
    /// <summary>
    /// This is entry point of program.
    /// </summary>

    void Start()
    {
        taker = players[0];
        turnSequencer = FindObjectOfType<TurnSequencer>();
        turnSequencer.Init(this);

        dealingSoundSource = GetComponent<AudioSource>();
        dealingSoundSource.clip = dealCardsProgress;
        tableImage = table.GetComponent<Image>();

        mainSource.clip = buttonClick;

        // add event listener to each button to play sound.
        Button[] btns = FindObjectsOfType<Button>();
        foreach (var btn in btns)
        {
            if(btn.gameObject.name != "UserAction")
                btn.onClick.AddListener(PlayButtonSound);            
        }      

        options = FindObjectOfType<Options>();
        options.Init(this);
        deck = FindObjectOfType<Deck>();
        deck.Init(this);
        ads = new ADS();
        

        ads.ShowBannerWhenReady();
    }   
    public void InitPlayers()
    {                        
        ActivePlayers.Clear();        
        BettingPlayers = 0;
        
        for (int i = 0; i < Config.MaxPlayers; i++)
        {
            
            if (i < Config.Players) 
            {
                ActivePlayers.Add(players[i]);
                //turn on active players game objects
                players[i].gameObject.SetActive(true);                
            }
            else
            {
                //turn off inactive players gameobjects
                players[i].gameObject.SetActive(false);
                //players[i].Role = Role.Defeated;
            }                
        }
       

        for (int i = 0; i < ActivePlayers.Count; i++)
        {
            //if player has been initialized, reset player.
            if (!ActivePlayers[i].Initialized)           
                ActivePlayers[i].Init(i, turnSequencer,this, Config.placingBetDelay, Config.takeDelay, options.CardBackground);
            
            ActivePlayers[i].ResetPlayer();

            if (!Config.specialCards)
                ActivePlayers[i].Cards = deck.GetCardsFromLiveDeckTasselated(i); //give activePlayers random cards
            else
                ActivePlayers[i].Cards = deck.GetSpecialCards(i); 

            ActivePlayers[i].StartReceivingCards();
        }
        //Recalculate turning order after live players list has been created.
        turnSequencer.RecalculateTurningOrder(ActivePlayers);
        
        PlayDealingSound();
        //test
        
        //activePlayers[0].MakeAllCardsHighestRank();
        //activePlayers[1].MakeAllCardsHighestRank();
        //activePlayers[2].MakeAllCardsHighestRank(); 
    }
    #endregion

    #region Game status
    public bool CheckVictoryConditions()
    {
        if (ActivePlayers[0].Role == Role.Defeated)
        {
            if (Config.Giveaway)
                Victory();
            else
                Defeat();
            return true;
        }

        //check if all m.ActivePlayers exept human
        bool allDefeated = true;
        bool oneDefeated = false;
        List<Player> defeated = new List<Player>();
        for (int i = 1; i < ActivePlayers.Count; i++)
        {
            if (ActivePlayers[i].Role != Role.Defeated)
                allDefeated = false;
            else
            {
                defeated.Add(ActivePlayers[i]);
                oneDefeated = true;
            }
        }
        //remove defeated players from active players.
        foreach (var item in defeated)
            ActivePlayers.Remove(item);
        if(oneDefeated)
            turnSequencer.RecalculateTurningOrder(ActivePlayers);

        if (allDefeated)
        {
            Victory();
            return true;
        }
        //check until one player eliminated condition
        //If it is true, then game is held until one player is defeated. Other m.ActivePlayers are winners.
        if (oneDefeated)
        {
            //giveaway game winner is the one who got rid of cards first.
            if (Config.Giveaway)
            {
                Defeat();
                return true;
            }

            if (Config.UntilOneEliminated)
            {
                Victory();
                return true;
            }            
        }
      
        return false;
    }
    public void UserAction()
    {
        // waiting for uset input. Make sure he can only draw card when it`s human player`s turn.
        if (ActivePlayers[0].Condition == Condition.Idle)
        {
            if (ActivePlayers[0].Role == Role.Better)            
                turnSequencer.CommenceBet();            
            else                                        
                turnSequencer.UserInput();
                   
        }
    }
    public void StartClassicGame()
    {               
        ResetRound();
    }
    public void StartEnhancedGame()
    {
        print("enhanced game started");
    }
    public void StartCampaignLevel(int level)
    {
        print("campaign level started");
    }
    public void Defeat()
    {
        PlaySound(defeat);
        ShowGameEndPanel(true, Config.DefeatMessage);   
    }
    public void Victory()
    {
        PlaySound(victory);
        ShowGameEndPanel(true, Config.VictoryMessage);
        firework.SetActive(true);
    }
    /// <summary>
    /// User Input
    /// </summary>
    public void RestartRound()
    {
        if (players[0].Condition != Condition.ReceivingCards)
        {
            ResetRound();
        }        
    }
    public void ResetRound()
    {
        inGame = true;
        BettingPlayers = 0;
        ShowGameEndPanel(false, "");
        deck.CreateLiveDeck();              
        InitPlayers();
        turnSequencer.ResetTS();
        firework.SetActive(false);
    }
    
    public void ExitToMainMenu()
    {
        foreach (var item in ActivePlayers)
        {
            item.ResetPlayer();
        }
        dealingSoundSource.Stop();

        ShowGameEndPanel(false, "");
        mainMenuCanvas.enabled = true;
        inGame = false;
        firework.SetActive(false);
    }
    private void ShowGameEndPanel(bool show,string text)
    {
        gameEndBackground.SetActive(show);
        gameEndStatusPanel.SetActive(show);
        gameEndText.text = text;
    }
    #endregion
    #region Firework
    [SerializeField]
    private GameObject firework;
    #endregion
    #region Quit game
    // some quit game logic
    float persistTime = 2f;
    float p = 0;
    bool escapeButtonPressed = false;
    bool inGame = false;
    [SerializeField]
    private GameObject confirmExitLabel = null;
    void Update()
    {
        //detect escape button pressed
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            //if user is in game then quit to main menu.
            if (inGame)
            {
                ExitToMainMenu();
                return;
            }

            if (escapeButtonPressed)
            {
                Application.Quit();                 
            }
            else
            {
                escapeButtonPressed = true;
                confirmExitLabel.SetActive(true);
            }
        }
            //reset timer, strikes when persistTime passes.
        if (escapeButtonPressed)
        {
            p += Time.deltaTime;
            if (p >= persistTime)
            {
                confirmExitLabel.SetActive(false);
                p = 0;
                escapeButtonPressed = false;
            }
        }
    }
    #endregion
}
