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
    [SerializeField] private Transform interactionParent; // Parent attached à câmera para segurar objetos interativos
    [SerializeField] private float moveDuration = 0.5f; // Duração da animação de movimento
    [SerializeField] private float destroyDuration = 0.5f; // Duração da animação de destruição

    private Camera playerCamera;
    private FirstPersonController playerController; // Referência ao controlador do jogador
    private bool isHoldingObject = false; // Flag para saber se o jogador está segurando um objeto

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>(); // Obtém a câmera filho deste objeto
        playerController = GetComponentInParent<FirstPersonController>(); // Obtém o PlayerController no parent
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

        // Cria um raycast a partir da câmera
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact(); // Chama a função de interação

                // Verifica e aplica o foco se necessário
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
                rb.isKinematic = true; // Desativa a física do objeto
            }

            // Move o objeto para o parent attached à câmera usando DOTween
            Transform objectTransform = collider.transform;
            objectTransform.SetParent(interactionParent);

            // Anima o objeto para a posição e rotação do parent
            objectTransform.DOLocalMove(Vector3.zero, moveDuration);
            objectTransform.DOLocalRotate(Vector3.zero, moveDuration);

            // Desativa o controlador do jogador
            if (playerController != null)
            {
                playerController.ToggleInteractionPlayer(false);
            }

            isHoldingObject = true; // Define que o jogador está segurando um objeto
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
                rb.isKinematic = false; // Reativa a física do objeto
            }

            // Remove o objeto do parent e reseta a posição
            heldObject.SetParent(null);

            // Reativa o controlador do jogador
            if (playerController != null)
            {
                playerController.ToggleInteractionPlayer(true);
            }

            isHoldingObject = false; // Define que o jogador não está mais segurando um objeto
        }
    }

    public void DestroyItem(Transform item, Action onDestroyItem = null)
    {
        // Impede que o jogador se mova enquanto a animação de destruição ocorre
        if (playerController != null)
        {
            playerController.ToggleInteractionPlayer(false);
        }

        // Escala o objeto para zero e desativa o objeto ao final da animação
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
    void Interact(); // Método de interação
}
