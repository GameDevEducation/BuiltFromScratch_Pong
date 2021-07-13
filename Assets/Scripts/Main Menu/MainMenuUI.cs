using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI VersionDisplay;
    [SerializeField] AudioMixer TargetMixer;
    [SerializeField] TMP_Dropdown GameModePicker;

    // Start is called before the first frame update
    void Start()
    {
        VersionDisplay.text = "v" + Application.version;

        TargetMixer.SetFloat(AudioMixerKeys.SFXVolume, PlayerPrefs.GetFloat(PlayerPrefKeys.SFXVolume, 0f));
        TargetMixer.SetFloat(AudioMixerKeys.BGMVolume, PlayerPrefs.GetFloat(PlayerPrefKeys.BGMVolume, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlay_HumanVsHuman()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.IsHumanVsHuman, 1);
        
        if (GameModePicker.value == 0)
            PlayerPrefs.SetInt(PlayerPrefKeys.IsWithItems, 0);
        else
            PlayerPrefs.SetInt(PlayerPrefKeys.IsWithItems, 1);

        PlayerPrefs.Save();

        SceneManager.LoadScene("Main Level");
    }

    public void OnPlay_HumanVsAI()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.IsHumanVsHuman, 0);

        if (GameModePicker.value == 0)
            PlayerPrefs.SetInt(PlayerPrefKeys.IsWithItems, 0);
        else
            PlayerPrefs.SetInt(PlayerPrefKeys.IsWithItems, 1);

        PlayerPrefs.Save();
        
        SceneManager.LoadScene("Main Level");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
