using CoreSystems.Extensions.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interact")]
    public bool currentlyInteractable;
    public bool oneTimeThrow;
    public UnityEvent interaction;
    public UnityEvent secondaryInteraction;
    public UnityEvent onThrow;
    public UnityEvent onDrop;
    [HideInInspector] public bool thrown;
    bool decision;



    [HideInInspector] public Vector3 spawnPosition;

    [Header("Pickable")]
    public bool currentlyPickable;
    public UnityEvent onGrab;

    [Header("Object Info")]
    public bool isGrabbed;
    private void Start()
    {
        decision = false;
        spawnPosition = transform.position;
    }

    public void SetInteractable(bool decision)
    {
        currentlyInteractable = decision;
    }
    public void IsGrabable(bool decision)
    {
        currentlyPickable = decision;
    }
    public void ToogleObj(GameObject obj)
    {
        decision = !decision;
        obj.SetActive(decision);
    }



    public void SetNewTransform(Transform obj)
    {
        this.transform.position = obj.position;
        this.transform.rotation = obj.rotation;
        this.transform.parent = obj.transform;
    }
    public void AnimationOnly_SetActiveFalse()
    {
        this.gameObject.SetActive(false);
    }

    public void TogglePickable(bool option)
    {
        this.currentlyPickable = option;
        this.GetComponent<Rigidbody>().freezeRotation = option;
        this.GetComponent<Rigidbody>().useGravity = option;
        this.GetComponent<Rigidbody>().isKinematic = !option;
    }

    
}
