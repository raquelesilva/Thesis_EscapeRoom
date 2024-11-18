// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using CoreSystems.Extensions.Attributes;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

#if UNITY_EDITOR
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;
    public static FirstPersonController instance;

    [Header("State")]
    public PlayerStates currentPlayerState;

    [Header("Camera")]
    #region Camera Movement Variables

    public Camera playerCamera;
    public Camera focusCamera;
    private PhysicsRaycaster focusCameraRaycast;

    [Header("Settings")]

    [SerializeField] bool cameraCanMove = true;
    [SerializeField, Disable("cameraCanMove")] float fov = 60f;
    [SerializeField, Disable("cameraCanMove")] bool invertCamera = false;
    [SerializeField, Disable("cameraCanMove")] public float mouseSensitivity = 2f;
    [SerializeField, Disable("cameraCanMove")] float maxLookAngle = 50f;
    [SerializeField, Disable("cameraCanMove")] LayerMask groundMask;

    [Header("Crosshair")]
    // Crosshair
    [SerializeField, Disable("cameraCanMove")] bool lockCursor = true;
    [SerializeField, Disable("cameraCanMove")] bool crosshair = true;
    [SerializeField, Disable("cameraCanMove")] Image crosshairImage;
    [SerializeField, Disable("cameraCanMove")] Color crosshairColor = Color.white;

    [Header("Flash Light")]
    // Crosshair
    [SerializeField, Disable("cameraCanMove")] bool canUseFlashLight = true;
    [SerializeField, Disable("cameraCanMove", "canUseFlashLight", "OR")] Light flashLight;
    [SerializeField, Disable("cameraCanMove", "canUseFlashLight", "OR")] float flashLightDistanceAdjustment = 6;


    [Header("Camera Zoom")]
    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    #endregion
    #endregion

    [Header("Movement")]
    #region Movement Variables

    [SerializeField] bool playerCanMove = true;
    [SerializeField, Disable("playerCanMove")] float walkSpeed = 5f;
    [SerializeField, Disable("playerCanMove")] float maxVelocityChange = 10f;

    // Internal Variables
    private bool isWalking = false;

    [Header("Sprint")]
    #region Sprint

    [SerializeField] bool enableSprint = true;
    [SerializeField, Disable("enableSprint")] bool unlimitedSprint = false;
    [SerializeField, Disable("enableSprint")] float sprintSpeed = 7f;
    [SerializeField, Disable("enableSprint", "unlimitedSprint", "OR")] float sprintDuration = 5f;
    [SerializeField, Disable("enableSprint", "unlimitedSprint", "OR")] float sprintCooldown = .5f;
    [SerializeField, Disable("enableSprint")] float sprintFOV = 80f;
    [SerializeField, Disable("enableSprint")] float sprintFOVStepTime = 10f;

    // Sprint Bar
    [SerializeField, Disable("enableSprint")] bool useSprintBar = true;
    [SerializeField, Disable("enableSprint")] bool hideBarWhenFull = true;
    [SerializeField, Disable("enableSprint")] Image sprintBarBG;
    [SerializeField, Disable("enableSprint")] Image sprintBar;
    [SerializeField, Disable("enableSprint")] float sprintBarWidthPercent = .3f;
    [SerializeField, Disable("enableSprint")] float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    [Header("Jump")]
    #region Jump

    [SerializeField] bool enableJump = true;
    [SerializeField, Disable("enableJump")] KeyCode jumpKey = KeyCode.Space;
    [SerializeField, Disable("enableJump")] float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded = false;

    #endregion

    [Header("Crouch")]
    #region Crouch

    [SerializeField] bool enableCrouch = true;
    [SerializeField, Disable("enableCrouch")] bool holdToCrouch = true;
    [SerializeField, Disable("enableCrouch")] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField, Disable("enableCrouch")] float crouchHeight = .75f;
    [SerializeField, Disable("enableCrouch")] float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    [Header("Head Bob")]
    #region Head Bob

    [SerializeField] bool enableHeadBob = true;
    [SerializeField, Disable("enableHeadBob")] Transform joint;
    [SerializeField, Disable("enableHeadBob")] public float bobSpeed = 10f;
    [SerializeField, Disable("enableHeadBob")] public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();

        StartCoroutine(UpdateFlashLight());

        crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        focusCameraRaycast = focusCamera.GetComponent<PhysicsRaycaster>();

        #region Sprint Bar

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
            {
                sprintBarCG.alpha = 0;
            }
        }
        else
        {
            //sprintBarBG.gameObject.SetActive(false);
            //sprintBar.gameObject.SetActive(false);
        }

        #endregion
    }

    float camRotation;

    public void SetMouseSens(float newMouseSens)
    {
        mouseSensitivity = newMouseSens;
    }

    private void Update()
    {
        if (currentPlayerState == PlayerStates.playing)
        {
            #region Camera

            // Control camera movement
            if (cameraCanMove)
            {
                var controllers = Input.GetJoystickNames();

                yaw = transform.localEulerAngles.y + cameraMove.x * mouseSensitivity;

                if (!invertCamera)
                {
                    pitch -= mouseSensitivity * cameraMove.y;
                }
                else
                {
                    // Inverted Y
                    pitch += mouseSensitivity * cameraMove.y;
                }

                // Clamp pitch between lookAngle
                pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

                transform.localEulerAngles = new Vector3(0, yaw, 0);
                playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
            }

            #region Camera Zoom

            if (enableZoom)
            {
                // Changes isZoomed when key is pressed
                // Behavior for toogle zoom
                if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
                {
                    if (!isZoomed)
                    {
                        isZoomed = true;
                    }
                    else
                    {
                        isZoomed = false;
                    }
                }

                // Changes isZoomed when key is pressed
                // Behavior for hold to zoom
                if (holdToZoom && !isSprinting)
                {
                    if (Input.GetKeyDown(zoomKey))
                    {
                        isZoomed = true;
                    }
                    else if (Input.GetKeyUp(zoomKey))
                    {
                        isZoomed = false;
                    }
                }

                // Lerps camera.fieldOfView to allow for a smooth transistion
                if (isZoomed)
                {
                    playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
                }
                else if (!isZoomed && !isSprinting)
                {
                    playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
                }
            }

            #endregion
            #endregion

            #region Sprint

            if (enableSprint)
            {
                if (isSprinting)
                {
                    isZoomed = false;
                    playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                    // Drain sprint remaining while sprinting
                    if (!unlimitedSprint)
                    {
                        sprintRemaining -= 1 * Time.deltaTime;
                        if (sprintRemaining <= 0)
                        {
                            isSprinting = false;
                            isSprintCooldown = true;
                        }
                    }
                }
                else
                {
                    // Regain sprint while not sprinting
                    sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
                }

                // Handles sprint cooldown 
                // When sprint remaining == 0 stops sprint ability until hitting cooldown
                if (isSprintCooldown)
                {
                    sprintCooldown -= 1 * Time.deltaTime;
                    if (sprintCooldown <= 0)
                    {
                        isSprintCooldown = false;
                    }
                }
                else
                {
                    sprintCooldown = sprintCooldownReset;
                }

                // Handles sprintBar 
                if (useSprintBar && !unlimitedSprint)
                {
                    float sprintRemainingPercent = sprintRemaining / sprintDuration;
                    sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
                }
            }

            #endregion

            #region Jump

            // Gets input and calls jump method
            if (enableJump && jumpPressed && isGrounded)
            {
                Jump();
                jumpPressed = false;
            }

            #endregion

            #region Crouch

            if (enableCrouch)
            {
                if (Input.GetKeyDown(crouchKey) && !holdToCrouch)
                {
                    Crouch();
                }

                if (Input.GetKeyDown(crouchKey) && holdToCrouch)
                {
                    isCrouched = false;
                    Crouch();
                }
                else if (Input.GetKeyUp(crouchKey) && holdToCrouch)
                {
                    isCrouched = true;
                    Crouch();
                }
            }

            #endregion

            CheckGround();

            if (enableHeadBob)
            {
                HeadBob();
            }
        }

    }
    private IEnumerator UpdateFlashLight()
    {
        if (canUseFlashLight)
        {
            RaycastHit hit;
            Physics.Raycast(flashLight.transform.position, flashLight.transform.forward, out hit, 9999);
            float distance = Vector3.Distance(flashLight.transform.position, hit.point);

            flashLight.intensity = distance * flashLightDistanceAdjustment;
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(UpdateFlashLight());
    }

    private Vector2 movement;
    private Vector2 cameraMove;
    private bool jumpPressed;
    private bool sprintPressed;

    public void OnMovement(InputAction.CallbackContext action)
    {
        movement = action.ReadValue<Vector2>();
    }

    public void OnCameraMove(InputAction.CallbackContext action)
    {
        cameraMove = action.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext action)
    {
        jumpPressed = action.performed;
    }

    public void OnSprint(InputAction.CallbackContext action)
    {
        sprintPressed = action.performed;
    }
    void FixedUpdate()
    {
        if (currentPlayerState == PlayerStates.playing)
        {
            Cursor.lockState = CursorLockMode.Locked;

            #region Movement

            if (playerCanMove)
            {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = new Vector3(movement.x, 0, movement.y);

                // Checks if player is walking and isGrounded
                // Will allow head bob
                if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
                {
                    isWalking = true;
                }
                else
                {
                    isWalking = false;
                }

                // All movement calculations shile sprint is active
                if (enableSprint && sprintPressed && sprintRemaining > 0f && !isSprintCooldown)
                {
                    targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = rb.linearVelocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    // Player is only moving when valocity change != 0
                    // Makes sure fov change only happens during movement
                    if (velocityChange.x != 0 || velocityChange.z != 0)
                    {
                        isSprinting = true;

                        if (isCrouched)
                        {
                            Crouch();
                        }

                        if (hideBarWhenFull && !unlimitedSprint)
                        {
                            sprintBarCG.alpha += 5 * Time.deltaTime;
                        }
                    }

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);

                }
                // All movement calculations while walking
                else
                {
                    isSprinting = false;

                    if (hideBarWhenFull && sprintRemaining == sprintDuration)
                    {
                        sprintBarCG.alpha -= 3 * Time.deltaTime;
                    }

                    targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = rb.linearVelocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
            }

            #endregion
        }
        else if (currentPlayerState == PlayerStates.paused)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.DrawRay(origin, direction * distance, Color.green);
                isGrounded = true;
            }
            else
            {
                Debug.DrawRay(origin, direction * distance, Color.red);
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if (isCrouched && !holdToCrouch)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        // Stands player up to full height
        // Brings walkSpeed back up to original speed
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed /= speedReduction;

            isCrouched = false;
        }
        // Crouches player down to set height
        // Reduces walkSpeed
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;

            isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if (isWalking)
        {
            // Calculates HeadBob speed during sprint
            if (isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (isCrouched)
            {
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }
            // Applies HeadBob movement
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }

    public void GotoRequiredPlace(Transform position)
    {
        playerCanMove = false;
        cameraCanMove = false;
        transform.position = position.position;

        playerCanMove = true;
        cameraCanMove = true;
        currentPlayerState = PlayerStates.playing;
    }

    #region Camera Animation
    private float cameraAnimationTime = 1f;

    public void SetAnimationSpeedInMS(float newSpeed)
    {
        cameraAnimationTime = newSpeed / 1000f; // Convert milliseconds to seconds
    }

    public void SetAnimationSpeedInSeconds(float newSpeed)
    {
        cameraAnimationTime = newSpeed; // Keep as seconds
    }


    public void TogglePhysicsRayCast(bool decision)
    {
        focusCameraRaycast.enabled = decision;
    }

    public void AnimateCamera(Transform spot)
    {
        if (playerCamera.gameObject.activeInHierarchy)
        {
            playerCamera.gameObject.SetActive(false);
            focusCamera.gameObject.SetActive(true);

            focusCamera.transform.position = playerCamera.transform.position;
            focusCamera.transform.rotation = playerCamera.transform.rotation;
            focusCamera.fieldOfView = playerCamera.fieldOfView;

            flashLight.transform.SetParent(focusCamera.transform, true);
            flashLight.transform.localPosition = Vector3.zero;
            flashLight.transform.localEulerAngles = Vector3.zero;
        }
        focusCamera.transform.SetParent(spot);
        StartCoroutine(MoveAndRotateToZero());
    }

    private IEnumerator MoveAndRotateToZero()
    {
        yield return new WaitForSeconds(.2f);

        Tween moveTween = focusCamera.transform.DOLocalMove(Vector3.zero, cameraAnimationTime).SetEase(Ease.InOutQuad);
        Tween rotateTween = focusCamera.transform.DOLocalRotate(Vector3.zero, cameraAnimationTime).SetEase(Ease.InOutQuad);

        yield return moveTween.WaitForCompletion();
        yield return rotateTween.WaitForCompletion();
    }

    public void GoBackCamera()
    {
        playerCamera.gameObject.SetActive(true);
        focusCamera.gameObject.SetActive(false);

        focusCamera.transform.SetParent(joint, false);
        focusCamera.transform.localPosition = Vector3.zero;
        focusCamera.transform.localEulerAngles = Vector3.zero;

        flashLight.transform.SetParent(playerCamera.transform, true);
        flashLight.transform.localPosition = Vector3.zero;
        flashLight.transform.localEulerAngles = Vector3.zero;
    }
    #endregion


    #region States
    public void SetPlayerStateDelayed(int newPlayerState)
    {
        StartCoroutine(Routine());
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(.1f);
            SetPlayerState(newPlayerState);
        }
    }
    public void SetPlayerState(int newPlayerState)
    {
        currentPlayerState = (PlayerStates)newPlayerState;
        switch (currentPlayerState)
        {
            case PlayerStates.playing:
                Cursor.lockState = CursorLockMode.Locked;
                crosshairImage.gameObject.SetActive(true);
                Cursor.visible = false;
                break;
            case PlayerStates.paused:
                Cursor.lockState = CursorLockMode.None;
                crosshairImage.gameObject.SetActive(false);
                Cursor.visible = true;
                break;
            case PlayerStates.focused:
                Cursor.lockState = CursorLockMode.None;
                crosshairImage.gameObject.SetActive(false);
                Cursor.visible = true;
                break;
        }
    }
    #endregion
}
public enum PlayerStates
{
    playing = 1,
    paused = 2,
    focused = 3,
    none = 4
}