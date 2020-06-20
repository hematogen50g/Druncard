using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Suit { Spades,Clubs,Hearts,Diamonds}
public enum Rank { Two,Three,Four,Five,Six,Seven,Eight,Nine,Ten,Jack,Queen,King,Ace}
public struct CardSignature
{
    public Suit Suit { get; private set; }
    public Rank Rank { get; private set; }
    public CardSignature(Suit s, Rank r)
    {
        Suit = s; Rank = r;
    }
}
public class Deck : MonoBehaviour
{
    public List<Card> GetCardByName(List<CardSignature> cs)
    {
        List<Card> cards = new List<Card>();
        Card c;
        for (int j = 0; j < cs.Count; j++)
        {

            for (int i = 0; i < fullDeck.Count; i++)
            {
                c = fullDeck[i];
                if (c.Suit == cs[j].Suit && c.Rank == (int)cs[j].Rank)
                    cards.Add(c);
            }
        }
        return cards;
    }
    private Options options;
    private List<Card> fullDeck = new List<Card>();
    /// <summary>
    /// Deck that is being distributed among livePlayers, can be smaller than fullDeck.
    /// </summary>
    private List<Card> liveDeck = new List<Card>();
    [SerializeField]
    private Sprite[] deck1 = null;
    [SerializeField]
    private Sprite[] deck2 = null;
    Main m;
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private Transform cardParent;
    /// <summary>
    /// Simple shuffle removes random element and adds it back to the end of the list
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="iterations"> how many times</param>
    public List<Card> ShuffleCards(List<Card> cards)
    {
        //good way to shuffle
        //split input cards to two piles
        List<Card> pile1 = new List<Card>();
        List<Card> pile2 = new List<Card>();

        //print("Initial Cards count " + cards.Count);
        for (int i = 0; i < cards.Count / 2; i++)
        {
            pile1.Add(cards[i]);
        }
        for (int i = cards.Count / 2; i < cards.Count; i++)
        {
            pile2.Add(cards[i]);
        }
        //print("Pile 1 count" + pile1.Count);
        //print("Pile 2 count" + pile1.Count);

        cards.Clear();
        // shuffle pro style, frrrrrrtkk....
        int randPile = 0;
        int randCard = 0;

        while (pile1.Count > 0 || pile2.Count > 0)
        {
            //random 0 or 1, meaning first of second pile
            randPile = Random.Range(0, 2);

            if (randPile == 0 && pile1.Count > 0)
            {
                randCard = Random.Range(0, pile1.Count);
                cards.Add(pile1[randCard]);
                pile1.RemoveAt(randCard);
            }

            if (randPile == 1 && pile2.Count > 0)
            {
                randCard = Random.Range(0, pile2.Count);
                cards.Add(pile2[randCard]);
                pile2.RemoveAt(randCard);
            }
        }
        //print("Piles final count " + pile1.Count + " and " + pile2.Count);
        //set new table positions of cards.
        //print("Cards final count = " + cards.Count);
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].ResetTablePosition(i * Config.initialDeckCardSpacing);
            cards[i].GO.transform.SetAsFirstSibling();
        }

        return cards;
    }
    /// <summary>
    /// Get range of cards depending on player index
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public List<Card> GetCardsFromLiveDeck(int player,int livePlayers)
    {
        int amount = Config.Cards / livePlayers;
        int min = player * amount;

        List<Card> crds = new List<Card>();
        for (int i = min; i < min + amount; i++)
        {
            crds.Add(liveDeck[i]);
        }
        return crds;

    }
    /// <summary>
    /// Get every Nth card depending on player index
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public List<Card> GetCardsFromLiveDeckTasselated(int player)
    {
        List<Card> crds = new List<Card>();
        for (int i = player; i < liveDeck.Count; i += Config.Players)
        {
            crds.Add(liveDeck[i]);
        }
        return crds;
    }
    public class SpecialCards
    {
        public List<Card> cards = new List<Card>();        
    }
    private List<SpecialCards> spCards; 
    public List<Card> GetSpecialCards(int i)
    {
        return spCards[i].cards;        
    }
    public void CreateLiveDeck()
    {
        liveDeck.Clear();
        //lowest rank a card must have to be included into liveDeck based on how many cards we want to use in liveDeck


        for (int i = 0; i < Config.MaxCards; i++)
        {
            //set indices of liveDeck cards
            fullDeck[i].Index = i;
            //set card background
            fullDeck[i].SetBackground(options.CardBackground);
            fullDeck[i].ResetStats();
            fullDeck[i].ResetTablePosition(Config.initialDeckCardSpacing * i);
            fullDeck[i].GO.transform.SetAsFirstSibling();

            //set card face. For now there is only TWO types of card faces.
            switch (Config.CardsFace)
            {
                case 0:
                    fullDeck[i].SetFace(deck1[i]);
                    break;
                case 1:
                    fullDeck[i].SetFace(deck2[i]);
                    break;
            }

            if (fullDeck[i].Rank >= Config.LowestRank)
            {

                liveDeck.Add(fullDeck[i]);
                fullDeck[i].GO.SetActive(true);
            }
            else
            {
                fullDeck[i].GO.SetActive(false);
            }
        }
        // here comes one unpleasant thing - 52 cannot be divided by 3 so we got one extra card.
        //What should we do with this card? give to random player perhaps. or just discard it.

        int remainingCards = liveDeck.Count % Config.Players;
        for (int i = 0; i < remainingCards; i++)
        {
            liveDeck[liveDeck.Count - 1].GO.SetActive(false);
            liveDeck.RemoveAt(liveDeck.Count - 1);
            //print("extra");
        }

        //shuffle cards before distributing to livePlayers
        liveDeck = ShuffleCards(liveDeck);
        if (Config.specialCards) 
        { 
        //Create some special cards
        spCards = new List<SpecialCards>() { new SpecialCards(), new SpecialCards(), new SpecialCards(), new SpecialCards() };        

        spCards[0].cards.AddRange(GetCardByName(Config.cardSignatures1));
        spCards[1].cards.AddRange(GetCardByName(Config.cardSignatures2));
        spCards[2].cards.AddRange(GetCardByName(Config.cardSignatures3));
        spCards[3].cards.AddRange(GetCardByName(Config.cardSignatures4));
        }

    }  
    public void InitFullDeck()
    {
        #region Init full deck       
        //fullDeck = new List<Card>();

        //how many cards of one suit are in deck.
        int maxSuitLength = Config.MaxCards / Config.MaxSuits;

        //suit is -1 because cycle will immediately increment it at first iteration because rank will be zero.
        int suit = 0;

        for (int i = 0; i < Config.MaxCards; i++)
        {
            int rank = i % maxSuitLength;

            // init new card with respective rank.

            // instantiate card prefab.
            // set default face and background
            GameObject cardGO = Instantiate(cardPrefab, cardParent);

            Card c = new Card(rank, (Suit)suit, cardGO, deck2[i], options.CardBackground);

            fullDeck.Add(c);

            //print("Rank " + rank + "Suit " + suit.ToString());
            if (rank == maxSuitLength - 1)
            {
                suit++;
            }
        }

        
        // after full deck is created, create live deck. 
        #endregion
    }
    public void Init(Main main)
    {
        m = main;
        options = FindObjectOfType<Options>();
        InitFullDeck();
    }
}

