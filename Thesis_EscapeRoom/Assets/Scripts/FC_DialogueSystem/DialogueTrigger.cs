using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace FancyCrab.DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        public DialogueContainer dialogue;

        [Header("Start Options")]
        [Tooltip("Start from a specific index node (-1 to use start node)")]
        public int startIndex = -1;

        [Header("Events")]
        public UnityEvent onDialogueStart;
        public UnityEvent onDialogueEnd;

        [Header("Custom Trigger Events")]
        public List<StringEventPair> customTriggers = new List<StringEventPair>();

        [System.Serializable]
        public struct StringEventPair
        {
            public string triggerID;
            public UnityEvent onTrigger;
        }


        private bool hasTriggered = false;

        public void TriggerDialogue()
        {
            if (DialogueManager.Instance != null)
            {
                hasTriggered = true;
                onDialogueStart?.Invoke();

                // Determina o node inicial baseado no índice
                DialogueNode startNode = null;

                if (startIndex >= 0 && dialogue != null)
                {
                    startNode = dialogue.GetNodeByIndex(startIndex);
                    if (startNode == null)
                    {
                        Debug.LogWarning($"[FancyCrabStudios] Node with index {startIndex} not found. Using default start node.");
                    }
                }

                DialogueManager.Instance.StartDialogue(dialogue, OnDialogueComplete, this, startNode);
            }
            else
            {
                Debug.LogError("[FancyCrabStudios] No DialogueManager found in scene!");
            }
        }

        // Sobrecarga para iniciar com um índice específico
        public void TriggerDialogueFromIndex(int index)
        {
            if (DialogueManager.Instance != null && dialogue != null)
            {
                hasTriggered = true;
                onDialogueStart?.Invoke();

                DialogueNode startNode = dialogue.GetNodeByIndex(index);
                if (startNode == null)
                {
                    Debug.LogError($"[FancyCrabStudios] Node with index {index} not found!");
                    return;
                }

                DialogueManager.Instance.StartDialogue(dialogue, OnDialogueComplete, this, startNode);
            }
        }

        public void InvokeCustomTrigger(string triggerID)
        {
            foreach (var pair in customTriggers)
            {
                if (pair.triggerID == triggerID)
                {
                    pair.onTrigger?.Invoke();
                    return;
                }
            }

            Debug.LogWarning($"[FancyCrabStudios] No custom trigger found with ID: {triggerID}");
        }

        private void OnDialogueComplete()
        {
            onDialogueEnd?.Invoke();
        }

        public void ResetTrigger()
        {
            hasTriggered = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawIcon(transform.position, "DialogueTrigger Icon", true);
        }
    }
}