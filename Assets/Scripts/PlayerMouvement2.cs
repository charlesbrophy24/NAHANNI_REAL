using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMouvement2 : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public Camera playerCamera;  // Changed from Transform to Camera

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private HeadBobbing headBobbing;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Ensure the Camera component is assigned and retrieve HeadBobbing if it exists
        if (playerCamera != null)
        {
            headBobbing = playerCamera.GetComponent<HeadBobbing>();
        }
        else
        {
            Debug.LogError("Player Camera is not assigned.");
        }
    }

    void Update()
    {

        if (!IsOwner){

            playerCamera.gameObject.SetActive(false);

            return;

        }
        HandleMovement();
        HandleMouseLook();
        HandleHeadBobbing();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera around the X-axis
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player object around the Y-axis
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadBobbing()
    {
        if (controller.velocity.magnitude > 0.1f && isGrounded && headBobbing != null)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            headBobbing.Bob(controller.velocity.magnitude, isSprinting);
        }
        else if (headBobbing != null)
        {
            headBobbing.ResetPosition();
        }
    }
}
