using TMPro;
using UnityEngine;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Unity.FantasyKingdom
{
    public class DialogueManager : MonoBehaviour
    {
        private static DialogueManager instance;

        [Header("Dialogue UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject continueButton;

        [Header("Choices UI")]
        [SerializeField] private GameObject[] choices;
        private TextMeshProUGUI[] choicesTxt;

        private Story currentStory;
        public bool dialogueIsActive;

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

        public void EnterDialogue(TextAsset inkJSON)
        {
            FirstPersonController.instance.SetPause(true);
            
            currentStory = new Story(inkJSON.text);
            dialogueIsActive = true;
            dialoguePanel.SetActive(true);

            ContinueStory();
        }

        public void ExitDialogue()
        {
            FirstPersonController.instance.SetPause(false);
            
            FirstPersonController.instance.playerCanMove = true;
            FirstPersonController.instance.cameraCanMove = true;
            FirstPersonController.instance.enableZoom = true;

            dialogueIsActive = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        }

        public void ContinueStory()
        {
            if (currentStory.canContinue)
            {
                dialogueText.text = currentStory.Continue();

                DisplayChoices();
            }
            else
            {
                ExitDialogue();
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
