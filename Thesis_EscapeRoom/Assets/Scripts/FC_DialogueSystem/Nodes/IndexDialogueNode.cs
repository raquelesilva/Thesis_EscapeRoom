using UnityEngine;

namespace FancyCrab.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Index Node", menuName = StudioInfo.ASSET_MENU_PATH + "Index Node")]
    public class IndexDialogueNode : DialogueNode
    {
        [Header("Index Settings")]
        [Tooltip("The index value for this node")]
        public int indexValue = 0;

        [Header("Next Node")]
        [Tooltip("The node to go to when this index is triggered")]
        public DialogueNode nextNode;

        // Esconder campos desnecessários
        [HideInInspector] public new DialogueActor actor;
        [HideInInspector] public new AudioClip nodeAudio;
        [HideInInspector] public new string onNodeEnterEvent;
        [HideInInspector] public new string onNodeExitEvent;

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(name) || name == "New Index Node")
            {
                name = $"Index [{indexValue}]";
            }
        }

        public void UpdateName()
        {
            name = $"Index [{indexValue}]";
        }
    }
}