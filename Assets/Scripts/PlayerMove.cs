using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Air Control")]
    [SerializeField] private float groundAcceleration = 50f;
    [SerializeField] private float airAcceleration = 15f;
    [SerializeField] private float airControlMaxSpeed = 8f;

    [Header("Crouch")]
    [SerializeField] private float crouchingHeight = 1.0f;
    [SerializeField] private float crouchTransitionSpeed = 8f;
    [SerializeField] private LayerMask ceilingCheckMask = ~0;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookXLimit = 85f;

    [Header("Interection")]
    public float pushForce = 1;

    private CharacterController controller;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool crouchInput;
    private bool isCrouching;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float verticalRotation;

    private float standingHeight;
    private float standingCameraY;
    private float crouchingCameraY;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();

        standingHeight = controller.height;
        standingCameraY = cameraTransform.localPosition.y;
        crouchingCameraY = standingCameraY * (crouchingHeight / standingHeight);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Look.performed += OnLookPerformed;
        inputActions.Player.Look.canceled += OnLookCanceled;

        inputActions.Player.Jump.performed += OnJumpPerformed;

        inputActions.Player.Sprint.performed += OnSprintPerformed;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;

        inputActions.Player.Crouch.performed += OnCrouchPerformed;
        inputActions.Player.Crouch.canceled += OnCrouchCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Look.performed -= OnLookPerformed;
        inputActions.Player.Look.canceled -= OnLookCanceled;

        inputActions.Player.Jump.performed -= OnJumpPerformed;

        inputActions.Player.Sprint.performed -= OnSprintPerformed;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;

        inputActions.Player.Crouch.performed -= OnCrouchPerformed;
        inputActions.Player.Crouch.canceled -= OnCrouchCanceled;

        inputActions.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => moveInput = Vector2.zero;

    private void OnLookPerformed(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    private void OnLookCanceled(InputAction.CallbackContext ctx) => lookInput = Vector2.zero;

    private void OnJumpPerformed(InputAction.CallbackContext ctx) => TryJump();

    private void OnSprintPerformed(InputAction.CallbackContext ctx) => sprintInput = true;
    private void OnSprintCanceled(InputAction.CallbackContext ctx) => sprintInput = false;

    private void OnCrouchPerformed(InputAction.CallbackContext ctx) => crouchInput = true;
    private void OnCrouchCanceled(InputAction.CallbackContext ctx) => crouchInput = false;

    private void Update()
    {
        HandleLook();
        HandleCrouch();
        HandleMove();
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -lookXLimit, lookXLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleCrouch()
    {
        bool wantsToCrouch = crouchInput;
        bool blockedAbove = isCrouching && !wantsToCrouch && IsCeilingBlocking();

        isCrouching = wantsToCrouch || blockedAbove;

        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        float targetCameraY = isCrouching ? crouchingCameraY : standingCameraY;

        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, crouchTransitionSpeed * Time.deltaTime);
        cameraTransform.localPosition = camPos;
    }

    private bool IsCeilingBlocking()
    {
        float checkDistance = standingHeight - controller.height;
        Vector3 origin = transform.position + Vector3.up * controller.height;
        return Physics.Raycast(origin, Vector3.up, checkDistance + 0.05f, ceilingCheckMask);
    }

    private void HandleMove()
    {
        bool isGrounded = controller.isGrounded;

        Vector3 wishDir = transform.right * moveInput.x + transform.forward * moveInput.y;

        float targetSpeed = isCrouching ? crouchSpeed : (sprintInput ? sprintSpeed : walkSpeed);

        if (isGrounded)
        {
            Vector3 targetVelocity = wishDir * targetSpeed;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, groundAcceleration * Time.deltaTime);

            if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }
        else
        {
            Vector3 addVelocity = wishDir * airAcceleration * Time.deltaTime;
            Vector3 newVelocity = horizontalVelocity + addVelocity;

            float currentSpeedInWishDir = Vector3.Dot(horizontalVelocity, wishDir);
            if (currentSpeedInWishDir < airControlMaxSpeed)
            {
                horizontalVelocity = newVelocity;
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 fullVelocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(fullVelocity * Time.deltaTime);
    }

    private void TryJump()
    {
        if (controller.isGrounded && !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;

        if (hitRigidbody == null || hitRigidbody.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        hitRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}
