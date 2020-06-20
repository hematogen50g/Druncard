using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Campaign : MonoBehaviour
{
    [SerializeField]
    Main m = null;
    [SerializeField]
    Canvas canvas = null;
    [SerializeField]
    private GameObject[] levelButtons = null;

    private TMP_Text[] levelTextst = null;
    private Image[] levelLocks = null;

    public void Init()
    {
        levelTextst = new TMP_Text[levelButtons.Length];
        levelLocks = new Image[levelButtons.Length];

    }
    public void Back()
    {
        canvas.enabled = false;
    }
    public void PlayLevel(int level)
    {
        m.StartCampaignLevel(level);
    }
}
