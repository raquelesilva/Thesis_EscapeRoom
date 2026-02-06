using System;
using UnityEngine;

namespace FancyCrab.CoreSystems.InteractionSystem
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Detect")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float detectDistance = 3f;
        [SerializeField] private LayerMask detectLayer = ~0;

        [Header("Messages")]
        [SerializeField, TextArea(1, 3)] private string msgEmpty = "";
        [SerializeField, TextArea(1, 3)] private string msgGrabAndInspect = "Press G to Grab\nPress I to Inspect";
        [SerializeField, TextArea(1, 3)] private string msgInspectOnly = "Press I to Inspect";
        [SerializeField, TextArea(1, 3)] private string msgInteract = "Press E to Interact";
        [SerializeField, TextArea(1, 3)] private string msgHolding = "Press G to Drop\nLeft Mouse to Throw\nPress I to Inspect";
        [SerializeField, TextArea(1, 3)] private string msgInspectMode = "Inspect Mode\nDrag Mouse to Rotate\nPress I or Esc to Exit";

        [Header("Input")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private KeyCode grabDropKey = KeyCode.G;
        [SerializeField] private KeyCode inspectKey = KeyCode.I;
        [SerializeField] private KeyCode exitInspectKey = KeyCode.Escape;

        [Header("Grab")]
        [SerializeField] private Transform grabPoint;
        [SerializeField] private float throwForce = 10f;

        [Header("Held Physics")]
        [SerializeField] private float holdForce = 80f;
        [SerializeField] private float holdDamping = 10f;
        [SerializeField] private float holdTorque = 80f;
        [SerializeField] private float holdAngularDamping = 10f;

        [Header("Inspect")]
        [SerializeField] private Transform inspectPoint;
        [SerializeField] private float inspectMoveSpeed = 18f;
        [SerializeField] private float inspectRotateSpeed = 220f;
        [SerializeField] private bool lockCursorWhileInspecting = true;
        [SerializeField] private bool allowScrollZoom = true;
        [SerializeField] private float zoomSpeed = 1.2f;
        [SerializeField] private float minInspectDistance = 0.2f;
        [SerializeField] private float maxInspectDistance = 2.0f;

        public event Action<string> OnDetectInterface;
        public event Action<bool> IsInspecting;

        private string lastMessage;

        private IGrabbable currentGrabbable;
        private GrabbableObject currentPickupObject;
        private IInteractable currentInteractable;
        private InteractableObject currentInteractableObject;
        private Rigidbody currentRigidbody;
        private Transform currentTransform;

        private IGrabbable heldGrabbable;
        private GrabbableObject heldPickupObject;
        private Rigidbody heldRigidbody;
        private Transform heldTransform;

        private bool heldOriginalUseGravity;
        private float heldOriginalDrag;
        private float heldOriginalAngularDrag;

        private bool isInspecting;

        private Rigidbody inspectRigidbody;
        private Transform inspectTransform;
        private IGrabbable inspectGrabbable;
        private GrabbableObject inspectPickupObject;

        private bool inspectWasHolding;
        private Vector3 inspectSavedPosition;
        private Quaternion inspectSavedRotation;

        private bool inspectOriginalUseGravity;
        private float inspectOriginalDrag;
        private float inspectOriginalAngularDrag;

        private float inspectDistanceToCamera;
        private float inspectYaw;
        private float inspectPitch;

        private void Awake()
        {
            if (playerCamera == null) playerCamera = Camera.main;
            if (inspectPoint != null && playerCamera != null)
                inspectDistanceToCamera = Vector3.Distance(playerCamera.transform.position, inspectPoint.position);
            else
                inspectDistanceToCamera = 0.6f;
        }

        private void Update()
        {
            if (isInspecting)
            {
                UpdateInspectInput();
                UpdateInspectTooltip();
                return;
            }

            DetectTarget();
            HandleInput();
        }

        private void FixedUpdate()
        {
            if (isInspecting)
            {
                UpdateInspectMove();
                return;
            }

            UpdateHeldPhysics();
        }

        private void DetectTarget()
        {
            if (heldGrabbable != null)
            {
                SendMessageIfChanged(msgHolding);
                return;
            }

            currentGrabbable = null;
            currentPickupObject = null;
            currentInteractable = null;
            currentInteractableObject = null;
            currentRigidbody = null;
            currentTransform = null;

            string message = msgEmpty;

            if (playerCamera == null)
            {
                SendMessageIfChanged(msgEmpty);
                return;
            }

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, detectDistance, detectLayer))
            {
                currentTransform = hit.collider.transform;
                currentRigidbody = hit.collider.attachedRigidbody;

                if (hit.collider.TryGetComponent(out IGrabbable grabbable))
                {
                    currentGrabbable = grabbable;
                    currentPickupObject = hit.collider.GetComponent<GrabbableObject>();

                    bool canGrab = currentPickupObject == null || currentPickupObject.CanGrab();
                    message = canGrab ? msgGrabAndInspect : msgInspectOnly;
                }
                else if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    currentInteractable = interactable;
                    currentInteractableObject = hit.collider.GetComponent<InteractableObject>();

                    bool canInteract = currentInteractableObject == null || currentInteractableObject.CanInteract();
                    if (canInteract) message = msgInteract;
                }
            }

            SendMessageIfChanged(message);
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(inspectKey))
            {
                if (heldRigidbody != null) StartInspectFromHeld();
                else if (currentRigidbody != null) StartInspectFromTarget();
                return;
            }

            if (Input.GetKeyDown(interactKey))
            {
                if (heldGrabbable == null && currentInteractable != null)
                {
                    bool canInteract = currentInteractableObject == null || currentInteractableObject.CanInteract();
                    if (canInteract) currentInteractable.OnInteract();
                }
            }

            if (Input.GetKeyDown(grabDropKey))
            {
                if (heldGrabbable != null) DropHeld();
                else if (currentGrabbable != null)
                {
                    bool canGrab = currentPickupObject == null || currentPickupObject.CanGrab();
                    if (canGrab) GrabCurrent();
                }
            }

            if (heldGrabbable != null && Input.GetMouseButtonDown(0))
            {
                ThrowHeld();
            }
        }

        private void GrabCurrent()
        {
            if (grabPoint == null) return;
            if (currentTransform == null) return;

            heldGrabbable = currentGrabbable;
            heldPickupObject = currentPickupObject;
            heldRigidbody = currentRigidbody;
            heldTransform = currentTransform;

            if (heldRigidbody == null)
            {
                ClearHeld();
                return;
            }

            heldOriginalUseGravity = heldRigidbody.useGravity;
            heldOriginalDrag = heldRigidbody.linearDamping;
            heldOriginalAngularDrag = heldRigidbody.angularDamping;

            heldRigidbody.useGravity = false;
            heldRigidbody.linearDamping = Mathf.Max(heldOriginalDrag, 6f);
            heldRigidbody.angularDamping = Mathf.Max(heldOriginalAngularDrag, 6f);

            heldTransform.SetParent(null, true);

            heldGrabbable.OnGrab();
            SendMessageIfChanged(msgHolding);
        }

        private void UpdateHeldPhysics()
        {
            if (heldRigidbody == null || grabPoint == null) return;

            Vector3 targetPos = grabPoint.position;
            Vector3 posError = targetPos - heldRigidbody.position;
            Vector3 accel = (posError * holdForce) - (heldRigidbody.linearVelocity * holdDamping);
            heldRigidbody.AddForce(accel, ForceMode.Acceleration);

            Quaternion targetRot = grabPoint.rotation;
            Quaternion rotError = targetRot * Quaternion.Inverse(heldRigidbody.rotation);

            rotError.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            if (!float.IsNaN(axis.x) && axis.sqrMagnitude > 0.0001f)
            {
                Vector3 torque = axis.normalized * (angle * Mathf.Deg2Rad * holdTorque) - (heldRigidbody.angularVelocity * holdAngularDamping);
                heldRigidbody.AddTorque(torque, ForceMode.Acceleration);
            }
        }

        private void DropHeld()
        {
            if (heldRigidbody == null)
            {
                ClearHeld();
                SendMessageIfChanged(msgEmpty);
                return;
            }

            RestoreHeldRigidbody();
            heldGrabbable?.OnDrop();

            ClearHeld();
            SendMessageIfChanged(msgEmpty);
        }

        private void ThrowHeld()
        {
            if (heldRigidbody == null)
            {
                ClearHeld();
                SendMessageIfChanged(msgEmpty);
                return;
            }

            RestoreHeldRigidbody();

            if (playerCamera != null)
                heldRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);

            heldGrabbable?.OnThrow();

            ClearHeld();
            SendMessageIfChanged(msgEmpty);
        }

        private void RestoreHeldRigidbody()
        {
            heldRigidbody.useGravity = heldOriginalUseGravity;
            heldRigidbody.linearDamping = heldOriginalDrag;
            heldRigidbody.angularDamping = heldOriginalAngularDrag;
        }

        private void ClearHeld()
        {
            heldGrabbable = null;
            heldPickupObject = null;
            heldRigidbody = null;
            heldTransform = null;
        }

        private void StartInspectFromHeld()
        {
            if (inspectPoint == null || playerCamera == null || heldRigidbody == null) return;

            IsInspecting?.Invoke(true);

            isInspecting = true;

            inspectWasHolding = true;
            inspectRigidbody = heldRigidbody;
            inspectTransform = heldTransform;
            inspectGrabbable = heldGrabbable;
            inspectPickupObject = heldPickupObject;

            inspectSavedPosition = inspectRigidbody.position;
            inspectSavedRotation = inspectRigidbody.rotation;

            inspectOriginalUseGravity = heldOriginalUseGravity;
            inspectOriginalDrag = heldOriginalDrag;
            inspectOriginalAngularDrag = heldOriginalAngularDrag;

            inspectRigidbody.useGravity = false;
            inspectRigidbody.linearVelocity = Vector3.zero;
            inspectRigidbody.angularVelocity = Vector3.zero;
            inspectRigidbody.linearDamping = Mathf.Max(inspectRigidbody.linearDamping, 8f);
            inspectRigidbody.angularDamping = Mathf.Max(inspectRigidbody.angularDamping, 10f);

            inspectDistanceToCamera = Mathf.Clamp(
                Vector3.Distance(playerCamera.transform.position, inspectPoint.position),
                minInspectDistance,
                maxInspectDistance
            );

            Vector3 e = inspectTransform.rotation.eulerAngles;
            inspectYaw = e.y;
            inspectPitch = e.x;

            if (lockCursorWhileInspecting)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SendMessageIfChanged(msgInspectMode);
        }

        private void StartInspectFromTarget()
        {
            if (inspectPoint == null || playerCamera == null || currentRigidbody == null || currentTransform == null) return;

            IsInspecting?.Invoke(true);

            isInspecting = true;

            inspectWasHolding = false;
            inspectRigidbody = currentRigidbody;
            inspectTransform = currentTransform;

            currentTransform.TryGetComponent(out inspectGrabbable);
            inspectPickupObject = currentTransform.GetComponent<GrabbableObject>();

            inspectSavedPosition = inspectRigidbody.position;
            inspectSavedRotation = inspectRigidbody.rotation;

            inspectOriginalUseGravity = inspectRigidbody.useGravity;
            inspectOriginalDrag = inspectRigidbody.linearDamping;
            inspectOriginalAngularDrag = inspectRigidbody.angularDamping;

            inspectRigidbody.useGravity = false;
            inspectRigidbody.linearVelocity = Vector3.zero;
            inspectRigidbody.angularVelocity = Vector3.zero;
            inspectRigidbody.linearDamping = Mathf.Max(inspectRigidbody.linearDamping, 8f);
            inspectRigidbody.angularDamping = Mathf.Max(inspectRigidbody.angularDamping, 10f);

            inspectDistanceToCamera = Mathf.Clamp(
                Vector3.Distance(playerCamera.transform.position, inspectPoint.position),
                minInspectDistance,
                maxInspectDistance
            );

            Vector3 e = inspectTransform.rotation.eulerAngles;
            inspectYaw = e.y;
            inspectPitch = e.x;

            if (lockCursorWhileInspecting)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            SendMessageIfChanged(msgInspectMode);
        }

        private void UpdateInspectInput()
        {
            if (Input.GetKeyDown(inspectKey) || Input.GetKeyDown(exitInspectKey))
            {
                EndInspect();
                return;
            }

            float mx = Input.GetAxisRaw("Mouse X");
            float my = Input.GetAxisRaw("Mouse Y");

            inspectYaw += mx * inspectRotateSpeed * Time.unscaledDeltaTime;
            inspectPitch -= my * inspectRotateSpeed * Time.unscaledDeltaTime;
            inspectPitch = Mathf.Clamp(inspectPitch, -85f, 85f);

            if (allowScrollZoom)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scroll) > 0.001f)
                {
                    inspectDistanceToCamera = Mathf.Clamp(
                        inspectDistanceToCamera - scroll * zoomSpeed,
                        minInspectDistance,
                        maxInspectDistance
                    );
                }
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                Quaternion targetRot = Quaternion.Euler(inspectPitch, inspectYaw, 0f);
                inspectRigidbody.MoveRotation(targetRot);
            }
        }

        private void UpdateInspectMove()
        {
            if (inspectRigidbody == null || playerCamera == null) return;

            Vector3 targetPos = playerCamera.transform.position + playerCamera.transform.forward * inspectDistanceToCamera;

            Vector3 toTarget = targetPos - inspectRigidbody.position;
            Vector3 vel = toTarget * inspectMoveSpeed - inspectRigidbody.linearVelocity * 2.5f;

            inspectRigidbody.AddForce(vel, ForceMode.Acceleration);
        }

        private void EndInspect()
        {
            if (inspectRigidbody != null)
            {
                inspectRigidbody.useGravity = inspectOriginalUseGravity;
                inspectRigidbody.linearDamping = inspectOriginalDrag;
                inspectRigidbody.angularDamping = inspectOriginalAngularDrag;
            }

            if (!inspectWasHolding && inspectRigidbody != null)
            {
                inspectRigidbody.position = inspectSavedPosition;
                inspectRigidbody.rotation = inspectSavedRotation;
                inspectRigidbody.linearVelocity = Vector3.zero;
                inspectRigidbody.angularVelocity = Vector3.zero;
            }

            if (lockCursorWhileInspecting)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            isInspecting = false;

            inspectRigidbody = null;
            inspectTransform = null;
            inspectGrabbable = null;
            inspectPickupObject = null;

            IsInspecting?.Invoke(false);

            if (heldGrabbable != null) SendMessageIfChanged(msgHolding);
            else SendMessageIfChanged(msgEmpty);
        }

        private void UpdateInspectTooltip()
        {
            SendMessageIfChanged(msgInspectMode);
        }

        private void SendMessageIfChanged(string message)
        {
            if (message == lastMessage) return;
            lastMessage = message;
            OnDetectInterface?.Invoke(message);
        }
    }
}
