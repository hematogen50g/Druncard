using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSequencer : MonoBehaviour
{
    #region Vars
    public List<Card> CardsOnTable = new List<Card>();
    private int turn = 0;
    private int readyPlayers = 0;    
    Main m;
    private Player taker;
    public Player TurningPlayer { get; set; }
    #endregion
    #region Initializing
    public void Init(Main main)
    {      
        m = main;        
    }
    /// <summary>
    /// Recalculate turning order. useful when active players list has been changed.
    /// </summary>
    /// <param name="ap"> Active players list</param>
    public void RecalculateTurningOrder(List<Player> ap)
    {
        turningOrderPlayers.Clear();
        //create players turning order list
        for (int j = 0; j < Config.cwTurningOrder.Length; j++)
        {
            //cycle through active players list.
            for (int i = 0; i < ap.Count; i++)
            {

                if (ap[i].Index == Config.cwTurningOrder[j])
                {
                    turningOrderPlayers.Add(ap[i]);
                    break;
                }
            }            
        }
    }
    public void ResetTS()
    {
        TurningPlayer = m.ActivePlayers[0];
        CardsOnTable.Clear();
        turn = readyPlayers = 0;
        sequenceBegun = sequencePaused = false;
    }
    #endregion
    #region Sequence
    private List<Player> turningOrderPlayers = new List<Player>();
    private bool sequenceBegun = false;
    private bool sequencePaused = false;
    public void BeginTurnSequence(Player firstToTurn)
    {
        turn = 0;
        TurningPlayer = firstToTurn;
        sequenceBegun = true;
        Invoke("Sequence", 0);        
    }
    public void UserInput()
    {
        if (!sequenceBegun)
        {
            BeginTurnSequence(m.players[0]);
            m.players[0].Condition = Condition.WaitingOtherPlayers;
        }
        if (sequencePaused)
        {
            m.players[0].Condition = Condition.WaitingOtherPlayers;
            ContinueTurnSequence();
        }
    }
    public void PauseTurnSequence()
    {
        sequencePaused = true;
    }
    public void ContinueTurnSequence()
    {
        sequencePaused = false;
        Invoke("Sequence", 0);
    }
    private void Sequence()
    {
        TurningPlayer.DrawCardFromTop();
        turn++;

        if (turn < turningOrderPlayers.Count)
        {
            TurningPlayer = GetNextTurningPlayer(TurningPlayer);
            if (TurningPlayer == m.players[0])
                PauseTurnSequence();
            else
            {
                Invoke("Sequence", Config.turnDelay);
            }
        }
        else
        {
            sequenceBegun = false;
        }
        
    }
    private Player GetNextTurningPlayer(Player turningPlayer)
    {
        int index = turningPlayer.Index;
        int nextIndex = 0;
        for (int i = 0; i < turningOrderPlayers.Count; i++)
        {
            //find current player position in turning order List
            if(index == turningOrderPlayers[i].Index)
            {
                //find next turning player index
                nextIndex = i + 1;
                break;
            }
        }

        if (nextIndex >= turningOrderPlayers.Count)
            nextIndex = 0;

        //print("Next turning player is "+turningOrderPlayers[nextIndex].Index);
        return turningOrderPlayers[nextIndex];
    }
    #endregion
    #region Calls   
   
    public void CommenceBet()
    {
        m.BettingPlayers = comparees.Count;
        for (int i = 0; i < comparees.Count; i++)
        {        
            comparees[i].BeginBet(Config.placingBetDelay*i);
        }
    }
    
    
    #endregion
    #region Callbacks
    public void EndTurn(Player player)
    {
        //print("Turn ended live m.ActivePlayers q = " + livePlayers.Count);
        CardsOnTable.Clear();
        m.BettingPlayers = 0;        
        turn = 0;
        for (int i = 0; i < m.ActivePlayers.Count; i++)
        {
            m.ActivePlayers[i].Condition = Condition.Idle;

            if (m.ActivePlayers[i].CardsLeft == 0)
            {
                m.ActivePlayers[i].Role = Role.Defeated;
            }
        }
        TurningPlayer = taker;
        // if there is no victory or loss, taker player turns if he is not human of course.
        if (!m.CheckVictoryConditions() )
        {           
            //if not human player begin turn sequence
            if (taker != m.players[0])
                BeginTurnSequence(taker);          
        }
    }
    
    public void DoneDrawingCard(Player plr)
    {       
        readyPlayers++;        
        if (m.BettingPlayers > 1)
        {
            if (readyPlayers == m.BettingPlayers)
            {
                readyPlayers = 0;
                // check if all betting players have been defeated while betting
                
                ComparePlayerCards();                                
            }
        }
        else if (readyPlayers == m.ActivePlayers.Count)
        {
            readyPlayers = 0;
            ComparePlayerCards();
        }
    }
    #endregion
    #region Comparator
    /// <summary>
    /// Returns m.ActivePlayers that are live and not betWatchers.
    /// </summary>
    /// <param name="p"></param>
    /// <returns>Partisipants,betters. </returns>
    private List<Player> CheckPartisipants(List<Player> p)
    {
        List<Player> parts = new List<Player>();
        List<Player> watchers = new List<Player>();
        foreach (var item in p)
            if (item != null)
                switch (item.Role)
                {
                    case Role.Partisipant:
                        parts.Add(item);
                        break;
                    case Role.Better:
                        parts.Add(item);
                        break;
                    case Role.BetWatcher:
                        watchers.Add(item);
                        break;
                }
        //print("Parts" + parts.Count + " Watchers" + watchers.Count);
        // check if  parts has any players in it. if not, it means that betters run out of cards so now betwatchers are compared.
        if (parts.Count == 0)
            return watchers;
        else
            return parts;
    }   
    private void SetRoles(List<Player> p, Role role)
    {
        foreach (var item in p)
            if (item != null)
                item.Role = role;
    }

    private List<Player> comparees = new List<Player>();
    private List<Player> betWatchers = new List<Player>();
    /// <summary>
    /// Compare m.ActivePlayers cards amongst participating or betting m.ActivePlayers.    
    /// </summary>    
    private void ComparePlayerCards()
    {
        // basically this manages situation when betting players run out of cards and there are betwatchers.
        comparees.Clear();        
        comparees = CheckPartisipants(m.ActivePlayers);
        //print("Comparees "+comparees.Count);
       
        //deal with lowest card beats ace situation, otherwise just find m.ActivePlayers with highest cards
        if (Config.TwoBeatsAce)
            comparees = ResolveTwoBeatsAceSituation(comparees);
        else
            comparees = FindPlayerWithHighestCard(comparees);

        // Here all m.ActivePlayers with scoring cards are found.
        if (comparees.Count == 1)
        {
            // here taking player is defined.
            comparees[0].TakeTrick();
            taker = comparees[0];
            SetRoles(m.ActivePlayers, Role.Partisipant);
        }
        else 
        {
            //This means there are more than one player with highest cardcommence bet.
            // All m.ActivePlayers become betwatchers, comparees become betters.
            SetRoles(m.ActivePlayers, Role.BetWatcher);
            SetRoles(comparees, Role.Better);

            //commence bet here.
            m.PlaySound(m.bet);
            //from here human player needs to control bet if such function is on and if human player is betting.
            //also if human player has no cards to bet, commence bet.
            if (m.ActivePlayers[0].Role == Role.Better && !Config.AutoBet && m.ActivePlayers[0].CardsLeft > 0)
            {
                m.ActivePlayers[0].Condition = Condition.Idle;               
            }
            else
            {                
                CommenceBet();
            }
        }       
    }
    /// <summary>
    /// Returns m.ActivePlayers with highest cards
    /// </summary>
    /// <param name="p"> list of m.ActivePlayers to compare</param>
    /// <returns></returns>
    private List<Player> FindPlayerWithHighestCard(List<Player> p)
    {
        List<Player> hPlayers = new List<Player>();
        int hCard = 0;
        //find out highest card among partisipants
        foreach (var item in p)
            if (item.DrawnCard != null && item.DrawnCard.Rank > hCard)
                hCard = item.DrawnCard.Rank;
        //add m.ActivePlayers with highest card to hPlayers list
        foreach (var item in p)
            if (item.DrawnCard != null && item.DrawnCard.Rank == hCard)
                hPlayers.Add(item);
        return hPlayers;
    }
    /// <summary>
    /// Returns scoring player in six/two beats ace situation. in this case scoring cards may be lowest or hihgest.
    /// </summary>
    /// <param name="p"></param>
    /// <returns>Scoring cards</returns>
    private List<Player> ResolveTwoBeatsAceSituation(List<Player> p)
    {
        //let`s check partisipating m.ActivePlayers
        List<Player> highest, middle, lowest;
        highest = new List<Player>();
        lowest = new List<Player>();
        middle = new List<Player>();

        
        //discover lowest and highest cardholders
        foreach (Player v in p)
        {
            if (v.DrawnCard.Rank == Config.HighestRank)
                highest.Add(v);
            else if (v.DrawnCard.Rank == Config.LowestRank)
                lowest.Add(v);
            else
                middle.Add(v);
        }                
        // here we got mix of highest, lowest and maybe medium cards.

        if (highest.Count > 0 && lowest.Count > 0)
        {            
            // lowest cards are scoring if there is no middle cards.
            //otherwise all cards are betting because they counter each other like paper, scissors and stone.
            if (middle.Count == 0)
                return lowest;
            else
                return p;
        }
        else
        {           
            //if there are no highest and lowest cards return m.ActivePlayers with highest cards
            return FindPlayerWithHighestCard(p);
        }
    }
   
    #endregion
    
}
