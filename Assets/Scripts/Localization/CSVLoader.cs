using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class CSVLoader
{
    private TextAsset csvFile;
    private char lineSeparator = '\n';
    private char surround = '"';
    private string[] fieldSeparator = { "\",\"" };
    
    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("Localization");
    }
    public Dictionary<string,string> GetDictionaryValues(string attributeId)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string[] lines = csvFile.text.Split(lineSeparator);
        int attributeIndex = -1;
        string[] headers = lines[0].Split(fieldSeparator, StringSplitOptions.None);

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains(attributeId))
            {
                attributeIndex = i;
                break;
            }
        }
        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            string[] fields = CSVParser.Split(line);

            for (int f = 0; f < fields.Length; f++)
            {
                fields[f] = fields[f].TrimStart(' ', surround);
                fields[f] = fields[f].TrimEnd('\r',surround);// this is crucial to remove '"' from last word in line               
            }

            if(fields.Length > attributeIndex)
            {
                var key = fields[0];

                if (dictionary.ContainsKey(key)) { continue; }

                var value = fields[attributeIndex];
                dictionary.Add(key, value);
            }
        }
        return dictionary;
    }
}
