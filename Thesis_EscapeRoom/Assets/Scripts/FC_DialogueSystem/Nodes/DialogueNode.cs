using UnityEngine;
using UnityEngine.Events;

namespace FancyCrab.DialogueSystem
{
    public abstract class DialogueNode : ScriptableObject
    {
        [HideInInspector] public string nodeGUID;
        [HideInInspector] public Vector2 position;

        [Header("Node Settings")]
        public DialogueActor actor;

        [Header("Node Index")]
        [Tooltip("Index/Order of this node in the dialogue")]
        public int nodeIndex = -1; // -1 significa não indexado

        [Tooltip("Optional audio to play when this node is displayed")]
        public AudioClip nodeAudio;

        [Header("Node Events")]
        [Tooltip("Event triggered when entering this node")]
        public UnityEvent onNodeEnter;

        [Tooltip("Event triggered when exiting this node")]
        public UnityEvent onNodeExit;

        // Mantendo compatibilidade com eventos por string (opcional)
        [Tooltip("Optional event name to trigger (legacy)")]
        public string onNodeEnterEvent;
        public string onNodeExitEvent;
    }
}