using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas optionsCanvas = null, tournamentCanvas = null, canvas = null;
    [SerializeField]
    private Main m = null;
    public void ShowOptions()
    {
        optionsCanvas.enabled = true;
    }
    public void ShowTournament()
    {
        tournamentCanvas.enabled = true;
    }
    public void StartClassicGame()
    {
        m.StartClassicGame();
        canvas.enabled = false;
    }
    public void StartEnhancedGame()
    {
        m.StartEnhancedGame();
        canvas.enabled = false;
    }
}
