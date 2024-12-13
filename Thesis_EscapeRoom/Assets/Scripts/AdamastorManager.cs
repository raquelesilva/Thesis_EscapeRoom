using CoreSystems.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class AdamastorManager : MonoBehaviour
    {
        [SerializeField] private int currentErrors = 0;
        [SerializeField] private int currentCorrect = 0;

        [SerializeField] List<string> hints = new List<string>();

        public static AdamastorManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void CheckAnswer(bool isCorrect)
        {
            if (isCorrect)
            {
                currentCorrect++;
                NotificationManager.instance.SetMessage("Isso Mesmo!", Color.green);
            }
            else
            {
                currentErrors++;
                GetHint(currentErrors);
                NotificationManager.instance.SetMessage("Hmm tenta novamente!", Color.red);
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
