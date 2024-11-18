using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    [Header("Components")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider sensibilitySlider;
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private TMP_Dropdown qualityDropDown;
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private Toggle fullScreenToggle;

    Resolution[] resolutions;

    
    /*
    private void Start()
    {
        sensibilitySlider.value = sensitivity;
        sensibilitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();


        int currentResoluctionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResoluctionIndex = i;
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResoluctionIndex;
        resolutionDropDown.RefreshShownValue();


       
    }

    private void OnEnable()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Music");
        sfxSlider.value = PlayerPrefs.GetFloat("SFX");
        qualityDropDown.value = PlayerPrefs.GetInt("qualityInt");
        fullScreenToggle.isOn = Screen.fullScreen;
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("gameVolume", volume);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("qualityInt", qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        Debug.Log("FullScreen");
    }

    public void SetResoluction(int resoluctionIndex)
    {
        Resolution resolution = resolutions[resoluctionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OnSensitivityChanged(float value)
    {
        sensitivity = value;
        Camera.main.GetComponent<FirstPersonController>().mouseSensitivity = sensitivity;
    }

    
    */
}
