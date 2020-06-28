using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof(TMP_Text))]
public class LocalizerUI : MonoBehaviour
{
    TMP_Text textField;
    public string key;
    void Awake()
    {
        textField = GetComponent<TMP_Text>();       
    }
    public void SetLocalizedValue()
    {

        string value = LocalizationSystem.GetLocalizedValue(key);
        textField.text = value;
    }
}
