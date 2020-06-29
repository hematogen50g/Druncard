using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Options menu controller
/// </summary>
public class Options : MonoBehaviour
{
    
    [SerializeField]
    private TMP_Text cardsButton = null, playersButton = null, twoBeats = null,
        twoBeatsButton = null, autoBetButton = null, untilOneEliminatedButton = null,giveawayButton = null;

    [SerializeField]
    private Image cardFace = null, cardBackground = null, table = null;
    [SerializeField]
    private Canvas canvas = null, infoCanvas = null;

    public Sprite[] faces = null, backgrounds = null, tables = null;
    public Sprite CardBackground { get { return backgrounds[Config.CardsBackground]; } }
    public Sprite[] flags;
    [SerializeField]
    private Image countryFlag;

    public void SwitchCards()
    {
        Config.SwitchVar(VarName.Cards);
        cardsButton.text = Config.Cards.ToString();
        //update lowest card beats ace.
        twoBeats.text = Config.LowestCardBeatsAce();
    }
    public void SwitchPlayers()
    {
        Config.SwitchVar(VarName.Players);
        playersButton.text = Config.Players.ToString();
    }
    public void SwitchTwoBeatsAce()
    {
        Config.SwitchVar(VarName.TwoBeatsAce);
        twoBeatsButton.text = Config.TwoBeatsAceString;
        SetTextColor(twoBeatsButton, Config.TwoBeatsAce);
    }
    public void SwitchAutoBet()
    {
        Config.SwitchVar(VarName.AutoBet);
        autoBetButton.text = Config.AutoBetString;
        SetTextColor(autoBetButton, Config.AutoBet);
    }
    public void SwitchCardsBackground()
    {
        Config.SwitchVar(VarName.Background);
        cardBackground.sprite = backgrounds[Config.CardsBackground];       
    }
    public void SwitchCardsFace()
    {
        Config.SwitchVar(VarName.Face);
        cardFace.sprite = faces[Config.CardsFace];
    }
    public void SwitchUntilOneEliminated()
    {
        Config.SwitchVar(VarName.UntilOneEliminated);
        untilOneEliminatedButton.text = Config.UntilOneEliminatedString;
        SetTextColor(untilOneEliminatedButton, Config.UntilOneEliminated);
    }
    public void SwitchGiveaway()
    {
        Config.SwitchVar(VarName.Giveaway);
        giveawayButton.text = Config.GiveawayString;
        SetTextColor(giveawayButton, Config.Giveaway);
    }
    public void SwitchTable()
    {
        Config.SwitchVar(VarName.Table);
        table.sprite = tables[Config.Table];
        main.SetTableSprite(table.sprite);
    }
    public void SwitchLanguage()
    {       
        Config.SwitchVar(VarName.Language);
        UpdateLanguageUI();
    }
    public void UpdateLanguageUI()
    {
        LocalizationSystem.language = (LocalizationSystem.Language)Config.Language;
        var uis = FindObjectsOfType<LocalizerUI>();
        //print(" UIS " + uis.Length);
        foreach (var item in uis)
        {
            //print(" loc key " + item.key);
            item.SetLocalizedValue();
        }
        // update all "yes" and "no"s
        autoBetButton.text = Config.AutoBetString;
        twoBeatsButton.text = Config.TwoBeatsAceString;
        untilOneEliminatedButton.text = Config.UntilOneEliminatedString;
        giveawayButton.text = Config.GiveawayString;

        countryFlag.sprite = flags[Config.Language];
    }
    /// <summary>
    /// If true sets white color else - red color
    /// </summary>
    /// <param name="text"></param>
    /// <param name="b"></param>
    private void SetTextColor(TMP_Text text,bool b)
    {
        if (b)
        {
            text.color = Color.white;
        }
        else
        {
            text.color = Color.red;
        }
    }
    public void Close()
    {
        canvas.enabled = false;
        WriteToPlayerPrefs();
    }
    public void CloseInfo()
    {
        infoCanvas.enabled = false;
    }
    public void ShowInfoCanvas()
    {
        infoCanvas.enabled = true;
    }
    public void ResetToDefaults()
    {
        Config.ResetToDefaults();
        ReadSettingsFromConfig();
        PlayerPrefs.DeleteAll();
        //print("Reset to defaults");
    }
    Main main;
    private void ReadSettingsFromConfig()
    {
        UpdateLanguageUI();

        cardFace.sprite = faces[Config.CardsFace];
        cardBackground.sprite = backgrounds[Config.CardsBackground];

        twoBeatsButton.text = Config.TwoBeatsAceString;
        playersButton.text = Config.Players.ToString();
        cardsButton.text = Config.Cards.ToString();

        twoBeats.text = Config.LowestCardBeatsAce();
        SetTextColor(twoBeatsButton, Config.TwoBeatsAce);


        autoBetButton.text = Config.AutoBetString;
        SetTextColor(autoBetButton, Config.AutoBet);

        untilOneEliminatedButton.text = Config.UntilOneEliminatedString;
        SetTextColor(untilOneEliminatedButton, Config.UntilOneEliminated);

        giveawayButton.text = Config.GiveawayString;
        SetTextColor(giveawayButton, Config.Giveaway);

        table.sprite = tables[Config.Table];
        //print("Read from config table "+Config.Table);
        main.SetTableSprite(table.sprite);
    }
    public void Init(Main m)
    {
        main = m;
        
        Config.Init(faces.Length, backgrounds.Length, tables.Length);
        ReadFromPlayerPrefs();
        ReadSettingsFromConfig();

    }
    #region PlayerPrefs
    private void ReadFromPlayerPrefs()
    {
        VarName v = 0;
        int i = 0;
        string varName = VarName.Players.ToString();
        // check language and set default
        if (!PlayerPrefs.HasKey(VarName.Language.ToString()))
        {
            //if user runs the game for the first time
            if (Application.systemLanguage ==  SystemLanguage.English)
                Config.SetVar(VarName.Language, 0);
            else if(Application.systemLanguage == SystemLanguage.Russian)
                Config.SetVar(VarName.Language, 1);
        }
        //iterate through enum.
        //while enum has such name.
        //when getting out of enum`s range varName will become integer without name.        
        while (varName != i.ToString())
        {
            int c;
            //check if there is such key, if not then player prefs does not exist. So leave default config values.
            if (PlayerPrefs.HasKey(varName))
            {
                c = PlayerPrefs.GetInt(varName);
                Config.SetVar(v, c);
                //print("Read " + c.ToString() + varName);
            }
            i++;
            v++;
            varName = ((VarName)i).ToString();
        }
    }
    public void WriteToPlayerPrefs()
    {
        PlayerPrefs.SetInt("Cards", Config.Cards);
        PlayerPrefs.SetInt("Players", Config.Players);
        PlayerPrefs.SetInt("Face", Config.CardsFace);
        PlayerPrefs.SetInt("Background", Config.CardsBackground);
        PlayerPrefs.SetInt("Table", Config.Table);
        PlayerPrefs.SetInt("Language", Config.Language);

        if (Config.TwoBeatsAce)
            PlayerPrefs.SetInt("TwoBeatsAce", 1);
        else
            PlayerPrefs.SetInt("TwoBeatsAce", 0);

        if (Config.AutoBet)
            PlayerPrefs.SetInt("AutoBet", 1);
        else
            PlayerPrefs.SetInt("AutoBet", 0);

        if (Config.UntilOneEliminated)
            PlayerPrefs.SetInt("UntilOneEliminated", 1);
        else
            PlayerPrefs.SetInt("UntilOneEliminated", 0);

        if (Config.Giveaway)
            PlayerPrefs.SetInt("Giveaway", 1);
        else
            PlayerPrefs.SetInt("Giveaway", 0);
    }
    #endregion
}
