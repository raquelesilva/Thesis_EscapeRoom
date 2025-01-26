using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

namespace Unity.FantasyKingdom
{
    public class eventWwise : MonoBehaviour
    {
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void OnMouseEnter()
        {
            PlayPickUP();
        }

        public void PlayPickUP()
        {
            AkSoundEngine.PostEvent("UI_Menu", gameObject);
        }
    }

  

}
