using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PickUpSystem : MonoBehaviour
{
    public static PickUpSystem instance;

    public Camera fpsCam;
    [Header("Pick Up Variables")]
    public Transform holdParent;
    public Vector3 holdPosition;
    public float pickUpRange = 5f;
    public float throwForce = 200f;
    public float moveForce = 250f;
    public float heldObjDrag = 10f;
    public string heldObjectTag;
    bool carrying = false;

    private GameObject heldObj;
    private RaycastHit hit;

    bool interactionButton;
    bool secondaryInteractionbButton;
    bool grabButton;
    bool throwButton;

    [Header("Put Down Settings")]
    [SerializeField] private LayerMask putDownLayerMask;

    [SerializeField] private float distanceToSeeRay;
    public bool interacting;

    private PromtManager promtManager;
    private FirstPersonController player;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        promtManager = PromtManager.instance;
        player = FirstPersonController.instance;
    }

    public enum ObjectState
    {
        grabable,
        interactable,
        putDown,
        both,
        none
    }

    [SerializeField] private ObjectState currentObjectState;
    void Update()
    {
        if (player.currentPlayerState == PlayerStates.focused)
        {
            currentObjectState = ObjectState.none;
            promtManager.TurnOffPromt();
            return;
        }



        #region PickUp System
        if (grabButton)
        {

            if (heldObj == null)
            {
                if (Physics.Raycast(fpsCam.gameObject.transform.position, fpsCam.gameObject.transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    var hitedObj = hit.transform.GetComponent<Interactable>();
                    if (hitedObj != null)
                    {
                        currentObjectState = ObjectState.grabable;
                        PickupObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                DropObject();
            }
            grabButton = false;
        }
        /*
        else if (throwButton && (heldObj != null))
        {
            ThrowObject();
            throwButton = false;
        }*/


        if (heldObj == null)
        {
            DetectInteractable();
        }
        else
        {
            DetectPutDown();
        }

        #endregion

        //Sistema de states de objectos

        switch (currentObjectState)
        {
            case ObjectState.grabable:
                promtManager.UpdateTooltip("Grab");
                break;
            case ObjectState.interactable:
                promtManager.UpdateTooltip("Interact");
                break;
            case ObjectState.putDown:
                promtManager.UpdateTooltip("PutDown");
                break;
            case ObjectState.both:
                promtManager.UpdateTooltip("InteractAndGrab");
                break;
            case ObjectState.none:
                promtManager.TurnOffPromt();
                break;
        }
    }

    public GameObject GetHeldObject()
    {
        return heldObj;
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactableObject = other.gameObject.GetComponent<Interactable>();
        if (interactableObject != null)
        {
            if (interactableObject.currentlyInteractable)
            {
                if (interactionButton)
                {
                    interactableObject.interaction.Invoke();
                    interactionButton = false;
                }
                if (secondaryInteractionbButton)
                {
                    interactableObject.secondaryInteraction.Invoke();
                    secondaryInteractionbButton = false;
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (heldObj != null) //Move Object
        {
            MoveObject();
        }
    }
    private void DetectPutDown()
    {/*
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, distanceToSeeRay, putDownLayerMask))
        {
            
            if (hit.collider.GetComponent<PutDownObjects>() != null)
            {
                var putDownObject = hit.collider.GetComponent<PutDownObjects>();

                if (putDownObject != null)
                {

                    if (putDownObject.CanInteract() && putDownObject.IsSuitable(heldObj))
                    {
                        currentObjectState = ObjectState.putDown;

                        if (interactionButton || secondaryInteractionbButton)
                        {
                            putDownObject.onInteract?.Invoke();
                            putDownObject.PutDown(heldObj);

                            Rigidbody heldRigidBody = heldObj.GetComponent<Rigidbody>();
                            heldObj.GetComponent<Interactable>().isGrabbed = false;
                            heldRigidBody.useGravity = true;
                            heldRigidBody.linearDamping = 1f * Time.deltaTime;
                            heldObjectTag = "";
                            carrying = false;
                            heldObj = null;

                            interactionButton = false;
                            gameObject.layer = LayerMask.NameToLayer("Player");

                            secondaryInteractionbButton = false;
                            currentObjectState = ObjectState.none;

                            return;
                        }
                        return;
                    }
                    else
                    {
                        currentObjectState = ObjectState.none;
                        return;
                    }
                }
                else
                {
                    currentObjectState = ObjectState.none;
                    return;
                }
            }
            else
            {
                currentObjectState = ObjectState.none;
            }

        }
        else
        {
            currentObjectState = ObjectState.none;

        }
        */
    }

    private void DetectInteractable()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit))
        {
            if (!carrying)
            {
                if (hit.collider.GetComponent<Interactable>())
                {
                    var interactableObject = hit.collider.GetComponent<Interactable>();


                    float distance = Vector3.Distance(hit.transform.position, fpsCam.transform.position);

                    if (distance <= pickUpRange && !carrying)
                    {
                        if (interactableObject.currentlyPickable && interactableObject.currentlyInteractable)
                        {

                            if (interactionButton)
                            {
                                interactableObject.interaction.Invoke();
                                interactionButton = false;
                            }
                            if (secondaryInteractionbButton)
                            {
                                interactableObject.secondaryInteraction.Invoke();
                                secondaryInteractionbButton = false;
                            }

                            currentObjectState = ObjectState.both;
                            return;
                        }
                        else if (interactableObject.currentlyPickable)
                        {
                            currentObjectState = ObjectState.grabable;
                        }
                        else if (interactableObject.currentlyInteractable)
                        {
                            //Assigns each unity event to each input
                            if (interactionButton)
                            {
                                interactableObject.interaction.Invoke();
                                interactionButton = false;
                            }
                            if (secondaryInteractionbButton)
                            {
                                interactableObject.secondaryInteraction.Invoke();
                                secondaryInteractionbButton = false;
                            }

                            currentObjectState = ObjectState.interactable;
                        }
                        else
                        {
                            currentObjectState = ObjectState.none;
                        }
                    }
                    else
                    {
                        currentObjectState = ObjectState.none;
                    }
                }
                else
                {
                    currentObjectState = ObjectState.none;
                    return;
                }
            }
        }
    }

    void MoveObject()
    {
        if (Vector3.Distance(heldObj.transform.position, holdParent.transform.position) > 0.1f)
        {
            Vector3 moveDiretion = (holdParent.position - heldObj.transform.position);
            heldObj.GetComponent<Rigidbody>().AddForce(moveDiretion * moveForce);
        }
    }
    public void PickupObject(GameObject pickObj)
    {
        if (pickObj == null) return;

        // Reset holdParent's position and rotation
        holdParent.transform.localPosition = holdPosition;
        holdParent.transform.localRotation = Quaternion.identity;

        Debug.Log("Player just grabbed: " + pickObj.name);

        Rigidbody objRigidBody = pickObj.GetComponent<Rigidbody>();
        Interactable interactable = pickObj.GetComponent<Interactable>();
        var pickable = interactable.currentlyPickable;
        this.gameObject.layer = LayerMask.NameToLayer("PlayerObsolete");

        if (objRigidBody && pickable)
        {
            interactable.onGrab?.Invoke();
            interactable.isGrabbed = true;

            objRigidBody.useGravity = false;
            objRigidBody.linearDamping = heldObjDrag * Time.timeScale;

            // Set parent and reset local position and rotation
            objRigidBody.transform.SetParent(holdParent, true);

            heldObjectTag = pickObj.gameObject.tag;
            heldObj = pickObj;
            carrying = true;
        }
    }
    public void DropObject()
    {
        Rigidbody heldRigidBody = heldObj.GetComponent<Rigidbody>();
        heldObj.GetComponent<Interactable>().onDrop.Invoke();
        heldObj.GetComponent<Interactable>().isGrabbed = false;
        heldRigidBody.useGravity = true;
        heldRigidBody.linearDamping = 1f; // Remove * Time.deltaTime
        heldObjectTag = "";
        carrying = false;
        heldObj.transform.parent = null;
        heldObj = null;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    void ThrowObject()
    {
        Rigidbody heldRigidbody = heldObj.GetComponent<Rigidbody>();
        var interactable = heldObj.GetComponent<Interactable>();
        heldObj.GetComponent<Interactable>().isGrabbed = false;
        if (interactable.oneTimeThrow && !interactable.thrown)
        {
            interactable.onThrow.Invoke();
            interactable.thrown = true;
        }
        else
        {
            interactable.onThrow.Invoke();
        }
        heldRigidbody.useGravity = true;
        heldRigidbody.linearDamping = 1f;
        heldObj.transform.parent = null;
        heldObj = null;
        heldRigidbody.AddForce(holdParent.forward * throwForce);
        Invoke(nameof(CarryingDelay), .2f);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void CarryingDelay() { carrying = false; }

    public void OnInteraction(InputAction.CallbackContext callBackContext)
    {
        interactionButton = callBackContext.performed;
    }
    public void OnSecondaryInteraction(InputAction.CallbackContext callBackContext)
    {
        secondaryInteractionbButton = callBackContext.performed;
    }
    public void OnGrab(InputAction.CallbackContext callBackContext)
    {
        grabButton = callBackContext.performed;
    }

    public void CallOnGrabManual()
    {
        grabButton = true;
        grabButton = false;
    }
    public void OnThrow(InputAction.CallbackContext callBackContext)
    {
        throwButton = callBackContext.performed;
    }

}
