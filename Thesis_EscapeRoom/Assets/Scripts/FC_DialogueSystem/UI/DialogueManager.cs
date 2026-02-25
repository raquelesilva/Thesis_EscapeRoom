using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FancyCrab.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("UI References - Fancy Crab Style")]
        public GameObject dialoguePanel;
        public Text actorNameText;
        public Text dialogueText;
        public Image actorImage;
        public Transform choicesContainer;
        public GameObject choiceButtonPrefab;

        [Header("Typing Effect")]
        public float typingSpeed = 0.05f;
        public bool skipTypingOnClick = true;
        public AudioClip typingSound;
        public AudioSource typingAudioSource;

        [Header("Settings")]
        public bool autoAdvance = false;
        public float autoAdvanceDelay = 3f;
        public KeyCode continueKey = KeyCode.Space;

        // Private variables
        private DialogueContainer currentDialogue;
        private DialogueNode currentNode;
        private DialogueTrigger currentTrigger; // Referência ao trigger que iniciou o diálogo
        private bool isTyping = false;
        private string fullText;
        private Action onDialogueEnd;
        private Coroutine autoAdvanceCoroutine;

        // Events
        public event Action<DialogueContainer> OnDialogueStarted;
        public event Action<DialogueContainer> OnDialogueEnded;
        public event Action<DialogueNode> OnNodeChanged;

        public static DialogueManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Modifique o método StartDialogue para aceitar um node inicial específico
        public void StartDialogue(DialogueContainer dialogue, Action onComplete = null, DialogueTrigger trigger = null, DialogueNode specificStartNode = null)
        {
            if (dialogue == null)
            {
                Debug.LogError("[FancyCrabStudios] Attempted to start null dialogue!");
                return;
            }

            currentDialogue = dialogue;
            currentTrigger = trigger;
            onDialogueEnd = onComplete;

            // Usa o node específico se fornecido, senão usa o start node do diálogo
            currentNode = specificStartNode != null ? specificStartNode : dialogue.startNode;

            if (currentNode == null)
            {
                Debug.LogError("[FancyCrabStudios] No start node found for dialogue!");
                return;
            }

            dialoguePanel.SetActive(true);

            // Trigger start event
            OnDialogueStarted?.Invoke(dialogue);

            // Play background music if specified
            if (dialogue.backgroundMusic != null)
            {
                // Implement your audio manager call here
            }

            DisplayNode(currentNode);
        }

        // No método DisplayNode, adicione tratamento para IndexDialogueNode:
        private void DisplayNode(DialogueNode node)
        {
            if (node == null)
            {
                EndDialogue();
                return;
            }

            // Invoke exit event on previous node
            if (currentNode != null)
            {
                currentNode.onNodeExit?.Invoke();

                if (!string.IsNullOrEmpty(currentNode.onNodeExitEvent) && currentTrigger != null)
                {
                    currentTrigger.InvokeCustomTrigger(currentNode.onNodeExitEvent);
                }
            }

            currentNode = node;

            // Invoke enter event on new node
            currentNode.onNodeEnter?.Invoke();

            OnNodeChanged?.Invoke(node);

            if (!string.IsNullOrEmpty(node.onNodeEnterEvent) && currentTrigger != null)
            {
                currentTrigger.InvokeCustomTrigger(node.onNodeEnterEvent);
            }

            // Update actor information
            if (node.actor != null)
            {
                actorNameText.text = node.actor.actorName;

                if (actorImage != null)
                    actorImage.sprite = node.actor.actorPortrait;
            }

            // Play node audio if specified
            if (node.nodeAudio != null && typingAudioSource != null)
            {
                typingAudioSource.PlayOneShot(node.nodeAudio);
            }

            // Clear previous choices
            ClearChoices();

            // Process based on node type
            if (node is TextDialogueNode textNode)
            {
                DisplayTextNode(textNode);
            }
            else if (node is ChoiceDialogueNode choiceNode)
            {
                DisplayChoiceNode(choiceNode);
            }
            else if (node is IndexDialogueNode indexNode)
            {
                // Index node apenas redireciona para o próximo node
                if (indexNode.nextNode != null)
                {
                    DisplayNode(indexNode.nextNode);
                }
                else
                {
                    Debug.LogWarning($"[FancyCrabStudios] Index node {indexNode.indexValue} has no next node. Ending dialogue.");
                    EndDialogue();
                }

            }
        }

        private void DisplayTextNode(TextDialogueNode textNode)
        {
            choicesContainer.gameObject.SetActive(false);

            StopAllCoroutines();
            fullText = textNode.dialogueText;
            StartCoroutine(TypeText());

            if (textNode.nextNode == null)
            {
                ShowContinueButton("End Dialogue");
            }
            else if (autoAdvance)
            {
                if (autoAdvanceCoroutine != null)
                    StopCoroutine(autoAdvanceCoroutine);
                autoAdvanceCoroutine = StartCoroutine(AutoAdvanceToNext(textNode.nextNode));
            }
        }

        private void DisplayChoiceNode(ChoiceDialogueNode choiceNode)
        {
            dialogueText.text = "";
            choicesContainer.gameObject.SetActive(true);

            foreach (var choice in choiceNode.Choices)
            {
                if (!string.IsNullOrEmpty(choice.choiceText))
                {
                    // Check if choice should be hidden based on flags
                    bool shouldShow = true;
                    if (!string.IsNullOrEmpty(choice.requiredFlag) && choice.hideIfFlagMissing)
                    {
                        // Check your flag system here
                        shouldShow = false; // Implement flag checking
                    }

                    if (shouldShow)
                    {
                        CreateChoiceButton(choice);
                    }
                }
            }
        }

        private void CreateChoiceButton(ChoiceDialogueNode.ChoiceOption choice)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            buttonText.text = choice.choiceText;

            button.onClick.AddListener(() => {
                if (autoAdvanceCoroutine != null)
                    StopCoroutine(autoAdvanceCoroutine);

                if (choice.nextNode != null)
                    DisplayNode(choice.nextNode);
                else
                    EndDialogue();
            });

            // Add fancy hover effect
            AddButtonEffects(button);
        }

        private void AddButtonEffects(Button button)
        {
            // Add your fancy button effects here
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1f, 0.8f, 0.2f); // FancyCrab gold
            button.colors = colors;
        }

        private IEnumerator TypeText()
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char letter in fullText.ToCharArray())
            {
                dialogueText.text += letter;

                // Play typing sound
                if (typingSound != null && typingAudioSource != null)
                {
                    typingAudioSource.PlayOneShot(typingSound);
                }

                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
        }

        private IEnumerator AutoAdvanceToNext(DialogueNode nextNode)
        {
            yield return new WaitForSeconds(autoAdvanceDelay);
            DisplayNode(nextNode);
        }

        private void ClearChoices()
        {
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private void ShowContinueButton(string buttonText = "Continue")
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonTextComponent = buttonObj.GetComponentInChildren<Text>();

            buttonTextComponent.text = buttonText;
            button.onClick.AddListener(EndDialogue);

            choicesContainer.gameObject.SetActive(true);
        }

        private void EndDialogue()
        {
            // Invoke exit event on current node
            if (currentNode != null)
            {
                currentNode.onNodeExit?.Invoke();

                // Legacy string event support
                if (!string.IsNullOrEmpty(currentNode.onNodeExitEvent))
                {
                    if (currentTrigger != null)
                    {
                        currentTrigger.InvokeCustomTrigger(currentNode.onNodeExitEvent);
                    }
                }
            }

            if (currentDialogue != null && !string.IsNullOrEmpty(currentDialogue.onDialogueEndEvent))
            {
                // Trigger dialogue end event
            }

            dialoguePanel.SetActive(false);
            OnDialogueEnded?.Invoke(currentDialogue);
            onDialogueEnd?.Invoke();

            currentTrigger = null; // Limpa a referência
        }

        private void Update()
        {
            if (!dialoguePanel.activeSelf) return;

            if (Input.GetKeyDown(continueKey))
            {
                HandleContinueInput();
            }
        }

        private void HandleContinueInput()
        {
            if (isTyping && skipTypingOnClick)
            {
                StopAllCoroutines();
                dialogueText.text = fullText;
                isTyping = false;
            }
            else if (!isTyping && currentNode is TextDialogueNode textNode)
            {
                if (textNode.nextNode != null)
                {
                    DisplayNode(textNode.nextNode);
                }
            }
        }
    }
}