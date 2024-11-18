using CoreSystems.Extensions.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultipurposeScript : MonoBehaviour
{
    [SerializeField] private bool interactable = true;

    [Header("Filter & Settings")]
    [SerializeField] List<string> filterTags;
    [SerializeField] bool useDistance = false;
    [SerializeField, Disable("useDistance")] float distance = 10;

    [Header("Mouse Events"), Space()]
    [SerializeField] private UnityEvent onMouseDown;
    [SerializeField] private UnityEvent onMouseUp;
    [SerializeField] private UnityEvent onMouseEnter;
    [SerializeField] private UnityEvent onMouseExit;
    [Header("Collision Events"), Space()]
    [SerializeField] private UnityEvent onCollisionEnter;
    [SerializeField] private UnityEvent onCollisionStay;
    [SerializeField] private UnityEvent onCollisionExit;
    [Header("Trigger Events"), Space()]
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;
    [SerializeField] private UnityEvent onTriggerStay;

    Camera playerCamera;
   

    private void Start()
    {
        playerCamera = FirstPersonController.instance.playerCamera;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!interactable) return;

        if (filterTags.Count == 0 || filterTags == null)
        {
            onCollisionEnter?.Invoke();
            return;
        }

        for (int i = 0; i < filterTags.Count; ++i)
        {
            if (collision.gameObject.CompareTag(filterTags[i]))
            {
                onCollisionEnter?.Invoke();
            }
            else if (string.IsNullOrEmpty(filterTags[i]))
            {
                onCollisionEnter?.Invoke();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!interactable) return;

        if (filterTags.Count == 0 || filterTags == null)
        {
            onCollisionExit?.Invoke();
            return;
        }

        for (int i = 0; i < filterTags.Count; ++i)
        {
            if (collision.gameObject.CompareTag(filterTags[i]))
            {
                onCollisionExit?.Invoke();
            }
            else if (string.IsNullOrEmpty(filterTags[i]))
            {
                onCollisionExit?.Invoke();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!interactable) return;

        if (filterTags.Count == 0 || filterTags == null)
        {
            onCollisionStay?.Invoke();
            return;
        }

        for (int i = 0; i < filterTags.Count; ++i)
        {
            if (collision.gameObject.CompareTag(filterTags[i]))
            {
                onTriggerStay?.Invoke();
            }
            else if (string.IsNullOrEmpty(filterTags[i]))
            {
                onTriggerStay?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!interactable) return;

        if (filterTags.Count == 0 || filterTags == null)
        {
            onTriggerEnter?.Invoke();
            return;
        }

        for (int i = 0; i < filterTags.Count; ++i)
        {
            if (other.gameObject.CompareTag(filterTags[i]))
            {
                onTriggerEnter?.Invoke();
            }
            else if (filterTags[i] == "" || string.IsNullOrEmpty(filterTags[i]))
            {
                onTriggerEnter?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!interactable) return;

        if (filterTags.Count == 0 || filterTags == null)
        {
            onTriggerExit?.Invoke();
            return;
        }

        for (int i = 0; i < filterTags.Count; ++i)
        {
            if (other.gameObject.CompareTag(filterTags[i]))
            {
                onTriggerExit?.Invoke();
            }
            else if (string.IsNullOrEmpty(filterTags[i]))
            {
                onTriggerExit?.Invoke();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!interactable) return;

        if (filterTags.Count == 0 || filterTags == null)
        {
            onTriggerStay?.Invoke();
            return;
        }

        for (int i = 0; i < filterTags.Count; ++i)
        {
            if (other.gameObject.CompareTag(filterTags[i]))
            {
                onTriggerStay?.Invoke();
            }
            else if (string.IsNullOrEmpty(filterTags[i]))
            {
                onTriggerStay?.Invoke();
            }
        }
    }

    private void OnMouseDown()
    {
        if (!interactable) return;

        if (useDistance)
        {
            if (Vector3.Distance(playerCamera.transform.position, transform.position) >= distance)
            {
                onMouseDown?.Invoke();
            }
        }
        else
        {
            onMouseDown?.Invoke();
        }
    }

    private void OnMouseUp()
    {
        if (!interactable) return;

        if (useDistance)
        {
            if (Vector3.Distance(playerCamera.transform.position, transform.position) >= distance)
            {
                onMouseUp?.Invoke();
            }
        }
        else
        {
            onMouseUp?.Invoke();
        }
    }

    private void OnMouseEnter()
    {
        if (!interactable) return;

        if (useDistance)
        {
            if (Vector3.Distance(playerCamera.transform.position, transform.position) >= distance)
            {
                onMouseEnter?.Invoke();
            }
        }
        else
        {
            onMouseEnter?.Invoke();
        }
    }

    private void OnMouseExit()
    {
        if (!interactable) return;

        if (useDistance)
        {
            if (Vector3.Distance(playerCamera.transform.position, transform.position) >= distance)
            {
                onMouseExit?.Invoke();
            }
        }
        else
        {
            onMouseExit?.Invoke();
        }
    }

    public void SetInteractable(bool decision)
    {
        interactable = decision;
    }
}