using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI SFXVolumeLabel;
    [SerializeField] Slider SFXVolume;
    [SerializeField] TextMeshProUGUI MusicVolumeLabel;
    [SerializeField] Slider MusicVolume;

    [SerializeField] AudioMixer TargetMixer;

    // Start is called before the first frame update
    void Start()
    {
        // retrieve the volumes
        float volume;
        if (TargetMixer.GetFloat(AudioMixerKeys.SFXVolume, out volume))
            SFXVolume.value = volume;
        if (TargetMixer.GetFloat(AudioMixerKeys.BGMVolume, out volume))
            MusicVolume.value = volume;

        // force refresh the UI
        OnSFXVolumeChanged(SFXVolume.value);
        OnMusicVolumeChanged(MusicVolume.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSFXVolumeChanged(float newVolume)
    {
        TargetMixer.SetFloat(AudioMixerKeys.SFXVolume, newVolume);

        float normalisedVolume = (newVolume - SFXVolume.minValue) / (SFXVolume.maxValue - SFXVolume.minValue);
        SFXVolumeLabel.text = "Sound Effect Volume [" + Mathf.RoundToInt(normalisedVolume * 100).ToString() + "%]";

        PlayerPrefs.SetFloat(PlayerPrefKeys.SFXVolume, newVolume);
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float newVolume)
    {
        TargetMixer.SetFloat(AudioMixerKeys.BGMVolume, newVolume);

        float normalisedVolume = (newVolume - MusicVolume.minValue) / (MusicVolume.maxValue - MusicVolume.minValue);
        MusicVolumeLabel.text = "Music Volume [" + Mathf.RoundToInt(normalisedVolume * 100).ToString() + "%]";

        PlayerPrefs.SetFloat(PlayerPrefKeys.BGMVolume, newVolume);
        PlayerPrefs.Save();
    }
}
