using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundsValuesPlayer : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Music", 1.0f);
        float savedSFX = PlayerPrefs.GetFloat("SFX", 1.0f);
        volumeSlider.value = savedVolume;
        sfxSlider.value = savedSFX;
    }

    public void OnVolumeChanges()
    {
        PlayerPrefs.SetFloat("Musix",volumeSlider.value);
    }
    public void OnSFXChanges()
    {
        PlayerPrefs.SetFloat("SFX", sfxSlider.value);
    }
}
