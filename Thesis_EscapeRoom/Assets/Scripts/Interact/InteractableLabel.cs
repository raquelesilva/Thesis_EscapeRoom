using TMPro;
using UnityEngine;

namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class InteractableLabel : MonoBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;
        [SerializeField] private TextMeshProUGUI labelText;

        private void OnEnable()
        {
            playerInteraction.OnDetectInterface += DisplayLabelCallback;
        }
        private void OnDisable()
        {
            playerInteraction.OnDetectInterface -= DisplayLabelCallback;
        }

        private void DisplayLabelCallback(string newLabel)
        {
            labelText.text = newLabel;
        }
    }
}