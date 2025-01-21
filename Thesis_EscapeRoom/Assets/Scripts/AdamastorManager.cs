using CoreSystems.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class AdamastorManager : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField] private int currentErrors = 0;
        [SerializeField] private int currentCorrect = 0;
        [SerializeField] List<string> hints = new List<string>();

        [Header("UI")]
        [SerializeField] GameObject helpWindow;
        [SerializeField] GameObject logContent;
        [SerializeField] TMP_InputField inputMessage;
        [SerializeField] TextMeshProUGUI message;
        [SerializeField] Button sendButton;

        [Header("Scripts")]
        [SerializeField] FirstPersonController firstPersonController;
        [SerializeField] OpenAICompanion openAICompanion;

        public static AdamastorManager instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            firstPersonController = FirstPersonController.instance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H) && !firstPersonController.GetIsGamePaused())
            {
                firstPersonController.SetPause(true);

                helpWindow.SetActive(true);
            }
        }

        public void SendQuestion()
        {
            AddToLog(inputMessage.text, true);

            openAICompanion.AskOpenAI(inputMessage.text);
            inputMessage.interactable = false;
            sendButton.interactable = false;
        }

        public void RecieveMessage(string AIMessage)
        {
            AddToLog(AIMessage, false);
            inputMessage.interactable = true;
            sendButton.interactable = true;
        }

        private void AddToLog(string messagemText, bool isPlayer)
        {
            GameObject currentMessage = Instantiate(message.gameObject, logContent.transform);

            if (isPlayer)
            {
                currentMessage.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
            }
            else
            {
                currentMessage.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
            }

            currentMessage.GetComponent<TextMeshProUGUI>().text = messagemText;
            currentMessage.SetActive(true);
        }

        public void CheckAnswer(bool isCorrect)
        {
            if (isCorrect)
            {
                currentCorrect++;
                NotificationManager.instance.SetMessage("Isso Mesmo!", Color.green, "win");
            }
            else
            {
                currentErrors++;
                GetHint(currentErrors);
                NotificationManager.instance.SetMessage("Hmm tenta novamente!", Color.red, "lose");
            }
        }

        public void ResetVariables()
        {
            currentErrors = 0;
        }

        public void GetHint(int errorsNum)
        {
            if (errorsNum % 2 == 0)
            {
                // Add AI answer here
                Debug.Log("You should be more careful with your answers");
            }
        }
    }
}
