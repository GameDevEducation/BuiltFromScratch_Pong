using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WinnerMessageDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSetWinText(int winnerNumber, int pointDifference)
    {
        WinnerMessageDisplay.text = "Player " + winnerNumber.ToString() + " wins by " + pointDifference.ToString() + " points!";
    }
}
