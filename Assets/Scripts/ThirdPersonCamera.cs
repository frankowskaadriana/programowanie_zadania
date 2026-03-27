using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 2, -5);
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Rotation Limits")]
    [SerializeField] private float minYAngle = -30f;
    [SerializeField] private float maxYAngle = 60f;

    private float currentXAngle = 0f;
    private float currentYAngle = 0f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Find player if not assigned
        if (playerTarget == null)
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Initialize angles
        Vector3 currentRotation = transform.eulerAngles;
        currentXAngle = currentRotation.y;
        currentYAngle = currentRotation.x;
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;

        HandleCameraInput();
        UpdateCameraPosition();
    }

    private void HandleCameraInput()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update angles
        currentXAngle += mouseX;
        currentYAngle -= mouseY;

        // Clamp vertical angle
        currentYAngle = Mathf.Clamp(currentYAngle, minYAngle, maxYAngle);
    }

    private void UpdateCameraPosition()
    {
        // Calculate desired rotation
        Quaternion targetRotation = Quaternion.Euler(currentYAngle, currentXAngle, 0);

        // Calculate desired position (offset rotated around the player)
        Vector3 desiredPosition = playerTarget.position + targetRotation * cameraOffset;

        // Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Look at the player
        transform.LookAt(playerTarget);
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}