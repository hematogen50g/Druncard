using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class LocalizationSystem
{
    public enum Language {English,Russian}
    public static Language language = Language.Russian;

    private static Dictionary<string, string> localizedENG;
    private static Dictionary<string, string> localizedRUS;

    private static string[] localizedNamesENG;
    private static string[] localizedNamesRUS;

    public static bool isInit = false;
    public static void Init()
    {
        CSVLoader loader = new CSVLoader();
        loader.LoadCSV();
        NamesLoader nLoader = new NamesLoader();
        nLoader.LoadCSV();

        localizedNamesENG = nLoader.GetNames(Language.English);
        localizedNamesRUS = nLoader.GetNames(Language.Russian);

        localizedENG = loader.GetDictionaryValues("eng");
        localizedRUS = loader.GetDictionaryValues("rus");
        isInit = true;
    }
    public static string[] GetLocalizedNames(Language lang)
    {
        if (!isInit) Init();

        switch (lang)
        {
            case Language.English:
                return localizedNamesENG;               
            case Language.Russian:
                return localizedNamesRUS;                           
        }
        return null;
    }
    public static string GetLocalizedValue(string key)
    {
        if (!isInit) Init();

        string value = key;

        switch (language)
        {
            case Language.English:
                localizedENG.TryGetValue(key, out value);
                break;
            case Language.Russian:
                localizedRUS.TryGetValue(key, out value);
                break;            
        }
        return value;
    }
}

