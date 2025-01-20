using TMPro;
using UnityEngine;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager instance;

        [Header("Dialogue UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI displayNameText;
        [SerializeField] private Image displayPortrait;
        [SerializeField] private GameObject continueButton;

        [Header("Choices UI")]
        [SerializeField] private GameObject[] choices;
        private TextMeshProUGUI[] choicesTxt;

        private Story currentStory;
        [SerializeField] private DialogueTrigger currentDialogueTrigger;
        public bool dialogueIsActive;

        private const string speakerTag = "speaker";
        private const string portraitTag = "portrait";
        private const string checkAnswerTag = "checkAnswer";
        private const string playEventTag = "playEvent";
        private bool answerValue;

        private void Awake()
        {
            if (instance == null) 
            {
                instance = this;
            }
        }

        public static DialogueManager GetInstance() 
        {
            return instance;
        }

        private void Start()
        {
            dialogueIsActive = false;
            dialoguePanel.SetActive(false);

            choicesTxt = new TextMeshProUGUI[choices.Length];
            int index = 0;

            foreach (GameObject choice in choices)
            {
                choicesTxt[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
                index++;
            }
        }

        private void Update()
        {
            if (dialogueIsActive)
            {
                return;
            }
        }

        public void EnterDialogue(TextAsset inkJSON, DialogueTrigger currentTrigger)
        {
            FirstPersonController.instance.SetPause(true);
            
            currentStory = new Story(inkJSON.text);
            currentDialogueTrigger = currentTrigger;
            dialogueIsActive = true;
            dialoguePanel.SetActive(true);

            // Reset layout, and speaker
            displayNameText.text = "????";

            ContinueStory();
        }

        public void ExitDialogue()
        {
            FirstPersonController.instance.SetPause(false);
            
            FirstPersonController.instance.playerCanMove = true;
            FirstPersonController.instance.cameraCanMove = true;
            FirstPersonController.instance.enableZoom = true;

            currentStory = null;
            currentDialogueTrigger = null;
            dialogueIsActive = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        }

        public void ContinueStory()
        {
            if (currentStory.canContinue)
            {
                string storyText = currentStory.Continue().Trim();

                // Check if the text is empty, and skip if it is
                if (!string.IsNullOrEmpty(storyText))
                {
                    dialogueText.text = storyText;

                    // Display choices and handle tags only if text exists
                    DisplayChoices();
                    HandleTags(currentStory.currentTags);
                }
                else
                {
                    // Automatically continue to the next line if it's empty
                    ContinueStory();
                }
            }
            else
            {
                ExitDialogue();
            }
        }

        private void HandleTags(List<string> currentTags)
        {
            foreach (var tag in currentTags)
            {
                string[] splitTag = tag.Split(':');
                if (splitTag.Length != 2)
                {
                    Debug.Log("Tag could not be appropriatly parsed: " + tag);
                }

                string tagName = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();
                
                switch (tagName)
                {
                    case speakerTag:
                        displayNameText.text = tagValue;
                        break;
                    case portraitTag:
                        Debug.Log("portrait = " + tagValue);
                        break;
                    case checkAnswerTag:
                        if (tagValue == "true")
                        {
                            answerValue = true;
                        }
                        else if (tagValue == "false")
                        {
                            answerValue = false;
                        }
                        else
                        {
                            Debug.LogError("Couldn't find the correct value!!");
                        }

                        AdamastorManager.instance.CheckAnswer(answerValue);
                        break;
                    case playEventTag:
                        currentDialogueTrigger.PlayEvents(tagValue);
                        break;
                    default:
                        Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                        break;
                }
            }
        }

        public void DisplayChoices()
        {
            List<Choice> currentChoices = currentStory.currentChoices;

            if (currentChoices.Count == 0)
            {
                continueButton.SetActive(true);
            }
            else
            {
                continueButton.SetActive(false);
            }

            if (currentChoices.Count > choices.Length)
            {
                Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
            }

            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].SetActive(false);
            }

            int index = 0;
            foreach (Choice choice in currentChoices)
            {
                Debug.Log(choice);
                choices[index].SetActive(true);
                choicesTxt[index].text = choice.text;
                index++;
            }

            StartCoroutine(SelectFirstChoice());
        }

        private IEnumerator SelectFirstChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
        }

        public void MakeChoice(int choiceIndex)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);

            ContinueStory();
        }
    }
}
