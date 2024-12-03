using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using TextAsset = UnityEngine.TextAsset;

namespace Unity.FantasyKingdom
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Ink JSON")]
        [SerializeField] private TextAsset inkJS;

        private bool playerInRange;

        private void Awake()
        {
            playerInRange = false;
        }

        private void Update()
        {
            if (playerInRange && !DialogueManager.GetInstance().dialogueIsActive && Input.GetKeyUp(KeyCode.E))
            {
                Debug.Log("I am in range");
                DialogueManager.GetInstance().EnterDialogue(inkJS);
            }   
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerInRange = false;

                DialogueManager.GetInstance().ExitDialogue();
            }
        }
    }
}