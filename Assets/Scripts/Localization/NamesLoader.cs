using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class NamesLoader
{   
    private string[][] langNames = new string[Enum.GetNames(typeof(LocalizationSystem.Language)).Length][];

    TextAsset csvFile;
    private bool loaded = false;
    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("Names");
        LoadNamesArray();
        loaded = true;
    }
    private void LoadNamesArray()
    {
        string[] nameStrings = csvFile.text.Split('-');
        //iterate through strings
        for (int i = 0; i < nameStrings.Length; i++)
        {
            string[] substring = nameStrings[i].Split(' ');
            for (int j = 0; j < substring.Length; j++)
            {
                substring[j] = substring[j].TrimStart();
                substring[j] = substring[j].TrimEnd('\r');
            }
            langNames[i] = substring;            
        }       
    }
    public string[] GetNames(LocalizationSystem.Language lang)
    {
        switch (lang)
        {
            case LocalizationSystem.Language.English:
                return langNames[0];
                
            case LocalizationSystem.Language.Russian:
                return langNames[1];
                           
        }
        return new string[]{"no names found" };
    }
}

