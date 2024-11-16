using Games.WiresGame;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class PliersScript : MonoBehaviour
{
    [SerializeField] private bool canInteract;
    [SerializeField] private LayerMask wirePlaneLayer;

    WiresGameManager gameManager;
    bool beingUsed;
    Animator animator;
    AudioSource sound;

    private void Start()
    {
        gameManager = WiresGameManager.instance;
        beingUsed = gameManager.UsingPliers();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    public void ToogleUsingPliers()
    {
        beingUsed = !beingUsed;
        gameManager.SetUsingPliers(beingUsed);

        if (!beingUsed)
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = new Vector3(0f, 180f, 90f);
        }
    }

    public void AnimatePlier()
    {
        animator.SetTrigger("Cut");
        sound.pitch = Random.Range(0.95f, 1.05f);
        sound.Play();
    }

    public void Update()
    {
        if (!beingUsed || !canInteract) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity, wirePlaneLayer))
        {
            transform.position = hitData.point;
            transform.LookAt(Camera.main.transform.position);
        }
    }

    public void SetInteractable(bool decision)
    {
        canInteract = decision;
    }
}
