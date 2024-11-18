using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    PlayerInput input;
    Camera mainCamera;
    RaycastHit rayHit;
    [SerializeField] private float distanceToSeeRay;
    public bool interacting;
    private bool canInteract;

    private PlayerInput _input;
    public static PlayerInteractions instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

        interacting = false;
        canInteract = false;
        mainCamera = FindObjectOfType<Camera>();
        
    }
    private void Update()
    {
        RaycastTest();
    }

    //This function uses a raycast to test if the object in front of the player is interactable or not
    void RaycastTest()
    {
        //Visual Representation of the Raycast in Scene View
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * distanceToSeeRay, Color.magenta);

        //Raycast detection
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out rayHit, distanceToSeeRay))
        {
            //Making sure the detection is getting the correct object
            Debug.Log("I hit" + rayHit.collider.gameObject.name);
           Interactable interactableObject = rayHit.collider.gameObject.GetComponent<Interactable>();
            //Tests to see if object is currently Interactable
            if (interactableObject != null)
            {
                if (interactableObject.currentlyInteractable)
                {
                    
                    canInteract = true;

                    //Assigns each unity event to each input
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactableObject.interaction.Invoke();
                    }
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        interactableObject.secondaryInteraction.Invoke();
                    }

                }
            }

        }
    }

    //This function serves the same purpose as the other one, but runs its tests with triggers instead of a raycast
    private void OnTriggerEnter(Collider other)
    {
        Interactable interactableObject = other.gameObject.GetComponent<Interactable>();
        if (interactableObject != null)
        {
            if (interactableObject.currentlyInteractable)
            {
                canInteract = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactableObject.interaction.Invoke();
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    interactableObject.secondaryInteraction.Invoke();
                }
            }
        }
    }
}
