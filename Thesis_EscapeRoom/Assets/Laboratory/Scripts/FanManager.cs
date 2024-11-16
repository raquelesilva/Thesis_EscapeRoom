using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

public class FanManager : MonoBehaviour
{
    [SerializeField] List<GameObject> correctImages;
    [SerializeField] GameObject fanButtons;
    [SerializeField] UnityEvent onUnlock;

    [SerializeField] TextMeshProUGUI statusText;

    [SerializeField] UnityEvent onWrong;
    [SerializeField] UnityEvent onCompleteGame;

    public void VerifyCode()
    {
        for (int i = 0; i < correctImages.Count; i++)
        {
            if (!correctImages[i].activeSelf)
            {
                onWrong?.Invoke();
                return;
            }
        }

        onUnlock?.Invoke();
    }

    public void TurnOnFan()
    {
        var currentLocale = LocalizationSettings.SelectedLocale;

        // Print out the locale's identifier, e.g., "en", "fr"
        string identifier = currentLocale.Identifier.ToString();

        switch (identifier)
        {
            case "pt":
                statusText.text = "Estado da ventilação: < color = green > Ligado </ color >";
                break;
            case "en":
                statusText.text = "Ventilation status: < color = green > Switched on </ color >";
                break;
            case "es":
                statusText.text = "Estado de ventilación: < color = green > Encendido </ color >";
                break;
            default:
                break;
        }

        onCompleteGame?.Invoke();
    }
}