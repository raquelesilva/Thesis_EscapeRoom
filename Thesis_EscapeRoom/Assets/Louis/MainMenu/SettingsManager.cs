using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [Header("Mixers")]
    [SerializeField] private AudioMixer masterMixer;

    private bool isMuted = false;

    [Header("Buttons Mute")]
    [SerializeField] private Button muteButton;
    [SerializeField] private Sprite muteIcon;
    [SerializeField] private Sprite unmuteIcon;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider dialogSlider;
    [SerializeField] private Slider sensibilitySlider;

    [Header("Others")]
    [SerializeField] private FirstPersonController player;

    private void Start()
    {
        LoadSettings();
    }

    public void LoadSettings()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Master", 1) * 100;
        musicSlider.value = PlayerPrefs.GetFloat("Music", 1) * 100;
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1) * 100;
        dialogSlider.value = PlayerPrefs.GetFloat("Dialog", 1) * 100;

        if (sensibilitySlider != null && player != null)
        {
            float playerSens = PlayerPrefs.GetFloat("Sens", 2);
            sensibilitySlider.value = playerSens;
            player.SetMouseSens(playerSens);
        }

        bool isMuted = PlayerPrefs.GetFloat("isMuted", 0) == 1;
        this.isMuted = isMuted;


        if (isMuted)
        {
            Mute();
        }
        else
        {
            Unmute();
        }
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    public void OnSFXValueChanged(float newValue)
    {
        newValue = newValue / 100;

        if (newValue < 0.01f)
        {
            newValue = 0.01f;
        }

        float volume = Mathf.Log10(newValue) * 30;
        PlayerPrefs.SetFloat("SFX", newValue);
        masterMixer.SetFloat("SFX", volume);
        muteButton.image.sprite = muteIcon;
        isMuted = false;
    }

    public void OnMusicValueChanged(float newValue)
    {
        newValue = newValue / 100;

        if (newValue < 0.01f)
        {
            newValue = 0.01f;
        }

        float volume = Mathf.Log10(newValue) * 30;
        PlayerPrefs.SetFloat("Music", newValue);
        masterMixer.SetFloat("Music", volume);
        muteButton.image.sprite = muteIcon;
        isMuted = false;
    }

    public void OnMasterValueChanged(float newValue)
    {
        newValue = newValue / 100;

        if (newValue < 0.01f)
        {
            newValue = 0.01f;
        }

        float volume = Mathf.Log10(newValue) * 30;
        PlayerPrefs.SetFloat("Master", newValue);
        masterMixer.SetFloat("Master", volume);
        muteButton.image.sprite = muteIcon;
        isMuted = false;
    }

    public void OnDialogValueChanged(float newValue)
    {
        newValue = newValue / 100;

        if (newValue < 0.01f)
        {
            newValue = 0.01f;
        }

        float volume = Mathf.Log10(newValue) * 30;
        PlayerPrefs.SetFloat("Dialog", newValue);
        masterMixer.SetFloat("Dialog", volume);
        muteButton.image.sprite = muteIcon;
        isMuted = false;
    }

    public void OnSensValueChange(float newValue)
    {
        if (player != null)
        {
            if (newValue < 0.2f)
            {
                newValue = 0.2f;
            }

            PlayerPrefs.SetFloat("Sens", newValue);
            player.SetMouseSens(newValue);
        }
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            Mute();
        }
        else
        {
            Unmute();
        }
    }

    private void Mute()
    {
        PlayerPrefs.SetFloat("isMuted", 1);

        masterMixer.SetFloat("Master", -80f);
        muteButton.image.sprite = unmuteIcon;
    }

    private void Unmute()
    {
        PlayerPrefs.SetFloat("isMuted", 0);

        float savedVolume = PlayerPrefs.GetFloat("Master", 1) * 100;
        savedVolume = savedVolume / 100;
        float volume = Mathf.Log10(savedVolume) * 30;
        masterMixer.SetFloat("Master", volume);
        muteButton.image.sprite = muteIcon;
    }
}
