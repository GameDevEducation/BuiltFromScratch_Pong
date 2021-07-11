using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Player1ScoreDisplay;
    [SerializeField] TextMeshProUGUI Player2ScoreDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnResetScores()
    {
        Player1ScoreDisplay.text = "00";
        Player2ScoreDisplay.text = "00";
    }

    public void OnChangePlayer1Score(int newScore)
    {
        Player1ScoreDisplay.text = newScore.ToString("0#");
    }

    public void OnChangePlayer2Score(int newScore)
    {
        Player2ScoreDisplay.text = newScore.ToString("0#");
    }
}
