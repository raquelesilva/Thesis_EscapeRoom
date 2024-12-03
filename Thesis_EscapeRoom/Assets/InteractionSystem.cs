using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem instance;

    [Header("Settings")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform interactionParent; // Parent attached � c�mera para segurar objetos interativos
    [SerializeField] private float moveDuration = 0.5f; // Dura��o da anima��o de movimento
    [SerializeField] private float destroyDuration = 0.5f; // Dura��o da anima��o de destrui��o

    private Camera playerCamera;
    private FirstPersonController playerController; // Refer�ncia ao controlador do jogador
    private bool isHoldingObject = false; // Flag para saber se o jogador est� segurando um objeto

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>(); // Obt�m a c�mera filho deste objeto
        playerController = GetComponentInParent<FirstPersonController>(); // Obt�m o PlayerController no parent
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isHoldingObject)
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void Interact()
    {
        RaycastHit hit;

        // Cria um raycast a partir da c�mera
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact(); // Chama a fun��o de intera��o

                // Verifica e aplica o foco se necess�rio
                ApplyFocus(hit.collider);
            }
        }
    }

    void ApplyFocus(Collider collider)
    {
        InteractableObject interactableObject = collider.GetComponent<InteractableObject>();
        if (interactableObject != null && interactableObject.FocusObject())
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Desativa a f�sica do objeto
            }

            // Move o objeto para o parent attached � c�mera usando DOTween
            Transform objectTransform = collider.transform;
            objectTransform.SetParent(interactionParent);

            // Anima o objeto para a posi��o e rota��o do parent
            objectTransform.DOLocalMove(Vector3.zero, moveDuration);
            objectTransform.DOLocalRotate(Vector3.zero, moveDuration);

            // Desativa o controlador do jogador
            if (playerController != null)
            {
                playerController.ToggleInteractionPlayer(false);
            }

            isHoldingObject = true; // Define que o jogador est� segurando um objeto
        }
    }

    void ReleaseObject()
    {
        // Solta o objeto atualmente segurado
        if (interactionParent.childCount > 0)
        {
            Transform heldObject = interactionParent.GetChild(0);
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false; // Reativa a f�sica do objeto
            }

            // Remove o objeto do parent e reseta a posi��o
            heldObject.SetParent(null);

            // Reativa o controlador do jogador
            if (playerController != null)
            {
                playerController.ToggleInteractionPlayer(true);
            }

            isHoldingObject = false; // Define que o jogador n�o est� mais segurando um objeto
        }
    }

    public void DestroyItem(Transform item, Action onDestroyItem = null)
    {
        // Impede que o jogador se mova enquanto a anima��o de destrui��o ocorre
        if (playerController != null)
        {
            playerController.ToggleInteractionPlayer(false);
        }

        // Escala o objeto para zero e desativa o objeto ao final da anima��o
        item.DOScale(Vector3.zero, destroyDuration).OnComplete(() =>
        {
            item.gameObject.SetActive(false); // Desativa o objeto
            ReleaseObject(); // Solta o objeto se estiver segurando
            if (playerController != null)
            {
                playerController.ToggleInteractionPlayer(true); // Reativa o controlador do jogador
            }
            if(onDestroyItem != null)
            {
                onDestroyItem?.Invoke();
            }
        });
    }
}

// Interface para objetos interativos
public interface IInteractable
{
    void Interact(); // M�todo de intera��o
}
