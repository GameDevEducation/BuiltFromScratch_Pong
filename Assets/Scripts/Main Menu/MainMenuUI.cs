using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI VersionDisplay;

    // Start is called before the first frame update
    void Start()
    {
        VersionDisplay.text = "v" + Application.version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlay_HumanVsHuman()
    {
        SceneManager.LoadScene("Main Level");
    }

    public void OnPlay_HumanVsAI()
    {
        SceneManager.LoadScene("Main Level");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
