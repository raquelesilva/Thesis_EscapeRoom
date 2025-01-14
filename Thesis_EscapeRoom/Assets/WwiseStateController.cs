using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

namespace Unity.FantasyKingdom
{
    public class WwiseStateController : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Awake()
        {
            AkSoundEngine.SetState("AmbienceState", "OnDeck");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AkSoundEngine.SetState("AmbienceState", "BelowDeck");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AkSoundEngine.SetState("AmbienceState", "OnDeck");
            }
        }
    }
}
