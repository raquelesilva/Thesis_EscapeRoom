using CoreSystems.Extensions.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCWindow : MonoBehaviour
{
    [SerializeField, ShowOnly] private bool isOpened;
    [SerializeField] private bool isStandAlone;

    private Animator animator;
    PCManager manager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        manager = GetComponentInParent<PCManager>();
    }

    public void ToggleWindow()
    {
        isOpened = !isOpened;
        if (!isOpened)
        {
            animator.SetTrigger("CloseApp");
        }
        else
        {
            gameObject.SetActive(true);
            gameObject.transform.SetAsLastSibling();
        }

        if (!isStandAlone)
        {
            manager.ChangeWindows(this);
        }
    }

    public void DisableWindow()
    {
        isOpened = false;
        if(animator != null)
        {
            animator.SetTrigger("CloseApp");
        }
    }
}
