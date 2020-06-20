using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Config
{
    #region Defaults
    const int players = 4;
    const int cards = 36;
    const bool untilOneEliminated = true;
    const bool giveaway = false;
    const bool autobet = false;
    const bool twoBeatsAce = true;
    const int face = 1;
    const int background = 0;
    const int table = 0;
    #endregion
    #region Game properties
    public static bool specialCards = false;
    public static List<CardSignature> cardSignatures1 = new List<CardSignature>() 
    { 
        new CardSignature(Suit.Clubs,Rank.Ace),
        //new CardSignature(Suit.Clubs,Rank.King),
       // new CardSignature(Suit.Clubs,Rank.Queen),
       // new CardSignature(Suit.Spades,Rank.Ace)
    };
    public static List<CardSignature> cardSignatures2 = new List<CardSignature>()
    {
        //new CardSignature(Suit.Diamonds,Rank.Ace),
        new CardSignature(Suit.Diamonds,Rank.King),
       // new CardSignature(Suit.Diamonds,Rank.Queen),
        //new CardSignature(Suit.Hearts,Rank.Two)
    };
    public static List<CardSignature> cardSignatures3 = new List<CardSignature>()
    {
      //  new CardSignature(Suit.Diamonds,Rank.Two),
       // new CardSignature(Suit.Diamonds,Rank.Five),
        new CardSignature(Suit.Diamonds,Rank.Jack),
        new CardSignature(Suit.Clubs,Rank.Five)
    };
    public static List<CardSignature> cardSignatures4 = new List<CardSignature>()
    {
        new CardSignature(Suit.Clubs,Rank.Jack),
       // new CardSignature(Suit.Spades,Rank.King),
       // new CardSignature(Suit.Spades,Rank.Queen),
      //  new CardSignature(Suit.Hearts,Rank.Eight)
    };



    public static int[] cwTurningOrder = { 0, 3, 1, 2 };
    public static int[] ccwTurningOrder = { 0, 2, 1, 3 };
    
    public const float cardMovingSpeed = 2.75f;
    public const float cardDistributingSpeed  = 1.5f;
    public const float turnDelay = 0.2f;
    public const float takeDelay = 0.85f;
    public const float placingBetDelay = 0.2f;
    public const float initialDeckCardSpacing = 2.5f;

    public const float ReceiveCardDelay = 0.13f;
    public const float XStep = 0.1f;
    public const float DeckShift = .025f;
    public const float YStep = .07f;
    public const float CardFlipSpeed = 11.5f;

    #endregion
}
