using CoreSystems.Managers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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

        [SerializeField] List<TriggerEvents> triggerEvents;

        private void Awake()
        {
            playerInRange = false;
        }

        private void Update()
        {
            if (playerInRange && !DialogueManager.GetInstance().dialogueIsActive && Input.GetKeyUp(KeyCode.E) && !FirstPersonController.instance.GetIsGamePaused())
            {
                Debug.Log("I am in range");
                DialogueManager.GetInstance().EnterDialogue(inkJS, this);
            }   
        }

        public void PlayEvents(string eventName)
        {
            foreach (var evnt in triggerEvents)
            {
                if (evnt.eventName == eventName)
                {
                    evnt.events.Invoke();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                NotificationManager.instance.SetMessage("I am close to you", Color.green);
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

        public void ChangeDialogue(TextAsset newInkJs)
        {
            inkJS = newInkJs;
        }
    }
}

[Serializable]
class TriggerEvents
{
    public string eventName;
    public UnityEvent events;
}