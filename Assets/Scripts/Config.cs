using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum VarName { Players,Cards,AutoBet,TwoBeatsAce,UntilOneEliminated,Giveaway,Face,Background,Table,Language}
public static partial class Config
{
    #region Developer
    public static bool ShowPlayerState { get; } = false;
    #endregion
    #region Words
    private static string Yes {get { return LocalizationSystem.GetLocalizedValue("yes"); }}
    private static string No { get { return LocalizationSystem.GetLocalizedValue("no"); } }
    public static string VictoryMessage { get { return LocalizationSystem.GetLocalizedValue("victory"); } }
    public static string DefeatMessage { get { return LocalizationSystem.GetLocalizedValue("defeat"); } }

    public static string LowestCardBeatsAce() 
    {
        if (Cards == MaxCards)
            return LocalizationSystem.GetLocalizedValue("two_beats_ace");
        else
            return LocalizationSystem.GetLocalizedValue("six_beats_ace");
    }
    private static int takenName;

    private static string[] PlayerNames
    {
        get { return LocalizationSystem.GetLocalizedNames(LocalizationSystem.language); }
    }
    public static string GetRandomPlayerName()
    {
        //this guarantees players names will be different from each other.
        int a = takenName;
        while(a==takenName)
            a = UnityEngine.Random.Range(0, PlayerNames.Length);
        takenName = a;
        return PlayerNames[a];
    }
    #endregion
    #region Common Properties

    public static int MinPlayers { get; private set; } = 2;
    public static int MaxPlayers { get; private set; } = 4;
    
    public static int MinCards { get; private set; } = 36;
    public static int  MaxCards { get; private set; } = 52;
    public static int MaxSuits { get; } = 4;
    
   
   
    public static string GiveawayString
    {
        get
        {
            if (Giveaway)
                return Yes;
            else
                return No;
        }
    }
    public static string UntilOneEliminatedString
    {
        get
        {
            if (UntilOneEliminated)
                return Yes;
            else
                return No;
        }
    }
    public static string TwoBeatsAceString
    {
        get
        {
            if (TwoBeatsAce)
                return Yes;
            else
                return No; 
        }
    }
    public static int LowestRank
    {
        get
        {
            return (MaxCards - Cards) / MaxSuits;
        }
    }
    public static int HighestRank
    {
        get
        {
            return MaxCards / MaxSuits - 1;
        }
    }
    public static string AutoBetString
    {
        get
        {
            if (AutoBet)
                return Yes;
            else 
                return No;
        }
    }

    //user defined properties
    public static int Players { get; private set; } = players;
    public static int Cards { get; private set; } = cards;
    public static bool TwoBeatsAce { get; private set; } = twoBeatsAce;
    public static bool AutoBet { get; private set; } = autobet;
    public static bool UntilOneEliminated { get; private set; } = untilOneEliminated;
    public static bool Giveaway { get; private set; } = giveaway;
    public static int CardsFace { get; private set; } = face;
    public static int CardsBackground { get; private set; } = background;
    public static int Table { get; private set; } = table;
    public static int Language { get; private set; } = language;

    private static int totalFaces, totalBackgrounds, totalTables,totalLanguages;

    #endregion
    public static void Init(int faces,int backgrounds,int tables)
    {
        totalFaces = faces;
        totalBackgrounds = backgrounds;
        totalTables = tables;
        totalLanguages = Enum.GetNames(typeof(LocalizationSystem.Language)).Length;
    }    
    public static void ResetToDefaults()
    {
        Language = language;
        Table = table;
        CardsBackground = table;
        CardsFace = table;
        Players = players;
        Cards = cards;
        TwoBeatsAce = twoBeatsAce;
        AutoBet = autobet;
        UntilOneEliminated = untilOneEliminated;
        Giveaway = giveaway;
    }
    /// <summary>
    /// Increases up to maximum value inclusive, then drops to minimum.
    /// </summary>
    /// <param name="a"> argument</param>
    /// <param name="min">min value</param>
    /// <param name="max">max value</param>
    /// <returns></returns>
    private static int Cycle(int a,int min,int max)
    {
        a++;
        if (a > max)
            a = min;
        if (a < min)
            a = max;
        return a;      
    }
      
    public static void SwitchVar(VarName name)
    {
        switch (name)
        {
            case VarName.Players:
                Players = Cycle(Players, MinPlayers, MaxPlayers);
                break;
            case VarName.Cards:
                if (Cards == MinCards)
                    Cards = MaxCards;
                else
                    Cards = MinCards;
                break;           
            case VarName.Face:
                CardsFace++;
                if (CardsFace >= totalFaces)
                    CardsFace = 0;

                break;
            case VarName.Background:

                CardsBackground++;
                if (CardsBackground >= totalBackgrounds)
                    CardsBackground = 0;

                break;
            case VarName.Table:                
                Table++;
                if (Table >= totalTables)
                    Table = 0;
                break;

            case VarName.Language:
                Language++;
                if (Language >= totalLanguages)
                    Language = 0;

                break;
            case VarName.AutoBet:
                AutoBet = !AutoBet;
                break;
            case VarName.TwoBeatsAce:
                TwoBeatsAce = !TwoBeatsAce;
                break;
            case VarName.UntilOneEliminated:
                UntilOneEliminated = !UntilOneEliminated;
                break;
            case VarName.Giveaway:
                Giveaway = !Giveaway;
                break;
            default:
                break;
        }
    }    
       
    public static void SetVar(VarName name, int val)
    {
        switch (name)
        {
            case VarName.Players:
                if (val >= MinPlayers && val <= MaxPlayers)
                    Players = val;
                break;
            case VarName.Cards:
                if (val >= MinCards && val <= MaxCards)
                    Cards = val;
                break;
            case VarName.AutoBet:
                if (val > 0)
                    AutoBet = true;
                else
                    AutoBet = false;
                break;
            case VarName.TwoBeatsAce:
                if (val > 0)
                    TwoBeatsAce = true;
                else
                    TwoBeatsAce = false;
                break;
            case VarName.UntilOneEliminated:
                if (val > 0)
                    UntilOneEliminated = true;
                else
                    UntilOneEliminated = false;
                break;
            case VarName.Giveaway:
                if (val > 0)
                    Giveaway = true;
                else
                    Giveaway = false;
                break;
            case VarName.Face:
                if (val < totalFaces && val >= 0)
                    CardsFace = val;
                break;
            case VarName.Background:
                if (val < totalBackgrounds && val >= 0)
                    CardsBackground = val;
                break;
            case VarName.Table:
                if (val < totalTables && val >= 0)
                    Table = val;
                break;
            case VarName.Language:
                if (val < totalLanguages && val >= 0)
                    Language = val;
                break;
        }
    }

    /// <summary>
    /// Returns index of next turning player
    /// </summary>
    /// <param name="index">current player index</param>
    /// <returns></returns>
   
    #region Tournaments
    #endregion
}
