using Michsky.UI.Shift;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LobbyInfoWindow : MonoBehaviour
{
    private FirstPersonController firstPersonController;

    [SerializeField] bool isDemo;
    int textsCount;

    [Header("TEXT")]
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField][TextArea(6, 6)] private string[] currentTexts;
    [SerializeField][TextArea(6, 6)] private List<MyInitialTexts> currentText;
    [SerializeField][TextArea(6, 6)] private List<MyInitialTexts> demoText;
    [SerializeField][TextArea(6, 6)] private List<MyInitialTexts> normalText;
    [SerializeField][TextArea(6, 6)] private string[] demoTexts;
    [SerializeField][TextArea(6, 6)] private string[] normalTexts;
    private int currentPage = 0;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("PAGING")]
    [SerializeField] private Image dot;
    [SerializeField] private Transform dotsLayout;
    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 highlightedColor;
    private List<Image> pageDots = new();

    private UIElementSound soundPlayer;

    private void Start()
    {
        firstPersonController = FindObjectOfType<FirstPersonController>();
        soundPlayer = GetComponent<UIElementSound>();

        if (isDemo)
        {
            currentTexts = demoTexts;
            currentText.Clear();
            for (int i = 0; i < demoText.Count; i++)
            {
                currentText[i] = demoText[i];
            }
        }
        else
        {
            currentText.Clear();
            for (int i = 0; i < normalText.Count; i++)
            {
                currentText[i] = normalText[i];
            }

            currentTexts = normalTexts;
        }

        textsCount = currentText[0].texts.Count;

        OpenWindow();
    }

    /// <summary>
    /// Opens window on Start and changes player state
    /// </summary>
    private void OpenWindow()
    {
        gameObject.SetActive(true);
        firstPersonController.SetPlayerState(2);

        for (int i = 0; i < textsCount; i++)
        {
            Image newDot = Instantiate(dot, dotsLayout);
            pageDots.Add(newDot);
        }

        for (int i = 0; i < currentTexts.Length; i++)
        {
            Image newDot = Instantiate(dot, dotsLayout);
            pageDots.Add(newDot);
        }

        currentPage = 0;
        UpdatePage();
    }

    public void NextPage()
    {
        soundPlayer.PlayClickSFX();
        currentPage++;
        UpdatePage();

        if (currentPage == textsCount - 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            previousButton.interactable = true;
            nextButton.interactable = true;
        }

        if (currentPage == currentTexts.Length - 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            previousButton.interactable = true;
            nextButton.interactable = true;
        }
    }

    public void PreviousPage()
    {
        soundPlayer.PlayClickSFX();
        currentPage--;
        UpdatePage();

        if (currentPage == 0)
        {
            previousButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
            previousButton.interactable = true;
        }
    }

    /// <summary>
    /// Updates both the text and the dots counter at the bottom
    /// </summary>
    private void UpdatePage()
    {
        UpdateText();
        UpdatePageDots();
    }

    private void UpdateText()
    {
        var currentLocale = LocalizationSettings.SelectedLocale;

        string identifier = currentLocale.Identifier.ToString();

        switch (identifier)
        {
            case "en":
                textBox.text = currentText[0].texts[currentPage];
                break;
            case "pt":
                textBox.text = currentText[1].texts[currentPage];
                break;
            case "es":
                textBox.text = currentText[3].texts[currentPage];
                break;
            default:
                break;
        }

        textBox.text = currentTexts[currentPage];
    }

    private void UpdatePageDots()
    {
        for (int i = 0; i < pageDots.Count; i++)
        {
            if (i <= currentPage)
            {
                pageDots[i].color = highlightedColor;
            }
            else
            {
                pageDots[i].color = normalColor;
            }
        }
    }

    [Serializable]
    public class MyInitialTexts
    {
        public string language;
        public List<string> texts;

        public MyInitialTexts(string language, List<string> texts)
        {
            this.language = language;
            this.texts = texts;
        }
    }
}