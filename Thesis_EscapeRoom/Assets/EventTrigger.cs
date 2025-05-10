using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

namespace Unity.FantasyKingdom
{
    
    

    public class PlayMusicOnTrigger : MonoBehaviour
    {
        public AK.Wwise.Event musicEvent; // Assign your Wwise Event in the Inspector

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) // Ensure the Player has the correct Tag
            {
                Debug.Log("Trigger Activated: Posting Wwise Event");
                musicEvent.Post(gameObject); // Posts the Event on this GameObject
            }
        }
    }
}
