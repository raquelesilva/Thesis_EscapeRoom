using CoreSystems.Extensions.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PutDownObjects : MonoBehaviour
{
    [SerializeField] bool canInteract;
    [Space(10)]

    [Header("Referencies")]
    [SerializeField] Transform placeToPutDown;
    [SerializeField] private float animationDuration;

    [Header("Filter")]
    [SerializeField] private bool hasFilter;
    [SerializeField, Disable("hasFilter")] public List<Interactable> correctItems;

    [Header("Events")]
    public UnityEvent onInteract;
    public UnityEvent onEndAnimation;

    GameObject currentHeldObject;
    PickUpSystem pickUpSystem;

    private void Start()
    {
        pickUpSystem = PickUpSystem.instance;
    }


    public void PutDown(GameObject heldObject)
    {
        if (placeToPutDown == null || currentHeldObject == null)
        {
            AnimateOBJ(heldObject);
        }
    }

    public void PutDownHoldingObject()
    {
        if (pickUpSystem.GetHeldObject() != null)
        {
            PutDown(pickUpSystem.GetHeldObject());
        }
    }

    public GameObject GetCurrentHeldObject()
    {
        return currentHeldObject;
    }

    public void SetAnimationSpeedInSeconds(float newSpeed)
    {
        animationDuration = newSpeed; // Keep as seconds
    }
    public void AnimateOBJ(GameObject heldObject)
    {
        heldObject.transform.SetParent(placeToPutDown, true);
        StartCoroutine(MoveAndRotateToZero(heldObject));
    }

    private IEnumerator MoveAndRotateToZero(GameObject heldObject)
    {
        yield return new WaitForSeconds(.2f);

        heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
        heldObject.GetComponent<Rigidbody>().useGravity = false;

        Tween moveTween = heldObject.transform.DOLocalMove(Vector3.zero, animationDuration).SetEase(Ease.InOutQuad);
        Tween rotateTween = heldObject.transform.DOLocalRotate(Vector3.zero, animationDuration).SetEase(Ease.InOutQuad);


        yield return moveTween.WaitForCompletion();
        yield return rotateTween.WaitForCompletion();
        heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.GetComponent<Rigidbody>().useGravity = true;

        onEndAnimation?.Invoke();
    }

    public bool CanInteract()
    {
        return canInteract;
    }
    public void SetCanInteract(bool decision)
    {
        canInteract = decision;
    }

    public bool IsSuitable(GameObject heldObject)
    {
        if (hasFilter)
        {
            foreach (var item in correctItems)
            {
                if (item.gameObject.Equals(heldObject.gameObject))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return true;
        }
    }
}