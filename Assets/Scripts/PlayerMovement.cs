using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundCheckDistance = 0.1f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayerMask = ~0;
    [SerializeField] private Transform groundCheckPoint;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform; // Assign the main camera

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float currentSpeed;

    // Input values
    private Vector2 moveInput;
    private bool isRunning;
    private bool jumpPressed;

    void Start()
    {
        // Get or add CharacterController
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 1f, 0);
        }

        // Get camera reference if not assigned
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("No main camera found! Movement will use world space.");
            }
        }

        // Create ground check point if not assigned
        if (groundCheckPoint == null)
        {
            GameObject checkPoint = new GameObject("GroundCheck");
            checkPoint.transform.SetParent(transform);
            checkPoint.transform.localPosition = new Vector3(0, -controller.height / 2 + 0.1f, 0);
            groundCheckPoint = checkPoint.transform;
        }
    }

    void Update()
    {
        if (controller == null) return;

        // Get input
        GetInput();

        // Check if grounded
        CheckGrounded();

        // Handle jumping
        HandleJump();

        // Apply gravity
        ApplyGravity();

        // Move the player
        MovePlayer();
    }

    private void GetInput()
    {
        // WASD movement
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.Normalize();

        // Running (Left Shift)
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Jumping (Space)
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckDistance, groundLayerMask);

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
    }

    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpPressed = false;
        }
        else
        {
            jumpPressed = false;
        }
    }

    private void ApplyGravity()
    {
        playerVelocity.y += gravity * Time.deltaTime;
    }

    private void MovePlayer()
    {
        // Determine movement speed
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Calculate camera-relative movement direction
        Vector3 moveDirection = GetCameraRelativeMovement();

        // Apply movement
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Apply vertical movement (gravity and jumping)
        controller.Move(playerVelocity * Time.deltaTime);

        // Optional: Rotate player to face movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private Vector3 GetCameraRelativeMovement()
    {
        if (cameraTransform == null)
        {
            // Fallback to world space if no camera
            return new Vector3(moveInput.x, 0, moveInput.y);
        }

        // Get camera's forward and right vectors
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten the vectors to ignore camera pitch (keep movement horizontal)
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize to maintain consistent speed even when camera is tilted
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to camera
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x);

        return moveDirection;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckDistance);
        }
    }
}