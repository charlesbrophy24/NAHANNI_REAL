using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 2f;
    public float jumpBoost = 2.2f;

    [Header("Zoom Settings")]
    public float zoomFOV = 40f;
    public float zoomSpeed = 10f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    [Header("Key Bindings")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode zoomKey = KeyCode.V;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    public Transform headTransform;

    private CharacterController characterController;
    private float defaultSpeed;
    private float defaultFOV;
    private bool isCrouching = false;
    private bool isZooming = false;
    private Vector3 velocity;
    private bool isGrounded;
    private float standingHeight;
    private float crouchingHeight;
    private Vector3 standingCenter;
    private Vector3 crouchingCenter;
    private Vector3 headStandingPosition;
    private Vector3 headCrouchingPosition;

    private bool hasMidAirCrouchBoosted = false;

    private float xRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (Camera.main != null)
        {
            defaultFOV = Camera.main.fieldOfView;
        }
        defaultSpeed = walkSpeed;
        standingHeight = characterController.height;
        crouchingHeight = 1f;
        standingCenter = characterController.center;
        crouchingCenter = new Vector3(characterController.center.x, characterController.center.y - 0.5f, characterController.center.z);

        if (headTransform != null)
        {
            headStandingPosition = headTransform.localPosition;
            headCrouchingPosition = new Vector3(headStandingPosition.x, headStandingPosition.y - 0.5f, headStandingPosition.z);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckGroundStatus();
        MovePlayer();
        HandleMouseLook();
        HandleZoom();
    }

    void CheckGroundStatus()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure player sticks to the ground
            hasMidAirCrouchBoosted = false; // Reset crouch boost when grounded
        }
    }

    [Header("Gravity Settings")]
    public float gravityMultiplier = 2f; // Multiplier for gravity strength
    public float fallMultiplier = 2.5f; // Extra gravity when falling

    void MovePlayer()
    {
        float moveSpeed = defaultSpeed;

        // Sprint and crouch adjustments
        if (Input.GetKey(sprintKey) && !isCrouching)
        {
            moveSpeed = sprintSpeed;
        }
        else if (Input.GetKey(crouchKey))
        {
            if (!isCrouching)
            {
                isCrouching = true;
                characterController.height = crouchingHeight;
                characterController.center = crouchingCenter; // Adjust center to match crouch
                AdjustCrouchPosition(true);
            }

            moveSpeed = crouchSpeed;
        }
        else if (isCrouching)
        {
            // Check for enough space to stand
            if (CanStandUp())
            {
                isCrouching = false;
                characterController.height = standingHeight;
                characterController.center = standingCenter; // Reset center to standing
                AdjustCrouchPosition(false);
            }
        }

        // Player movement
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * Time.deltaTime);

        // Jump logic
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * (Physics.gravity.y * gravityMultiplier));
        }

        // Apply gravity
        if (velocity.y < 0) // Falling
        {
            velocity.y += Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else // Normal gravity
        {
            velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }



    bool CanStandUp()
    {
        float rayLength = standingHeight - crouchingHeight;
        Vector3 rayOrigin = transform.position + Vector3.up * crouchingHeight;
        int layerMask = ~LayerMask.GetMask("Player");
        return !Physics.Raycast(rayOrigin, Vector3.up, rayLength, layerMask);
    }

    void AdjustCrouchPosition(bool crouching)
    {
        if (headTransform != null)
        {
            Vector3 targetPosition = crouching ? headCrouchingPosition : headStandingPosition;
            StartCoroutine(SmoothPositionChange(headTransform, targetPosition));
        }
    }

    IEnumerator SmoothPositionChange(Transform target, Vector3 targetPosition)
    {
        Vector3 startPosition = target.localPosition;
        float elapsed = 0f;
        float duration = 0.2f; // Smooth transition time
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }
        target.localPosition = targetPosition;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            isZooming = !isZooming;
        }
        if (Camera.main != null)
        {
            float targetFOV = isZooming ? zoomFOV : defaultFOV;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }
}
