using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    // UnityEvent que será disparado quando o objeto for interagido
    [SerializeField] UnityEvent onInteract;
    [SerializeField] bool focusObject;
    
    Outline outline;
    bool animatingOutline;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void Interact()
    {
        onInteract?.Invoke();
    }

    public bool FocusObject() => focusObject;
}
