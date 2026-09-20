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
    [SerializeField] private float groundAcceleration = 50f;   // 지상: 즉각 반응
    [SerializeField] private float airAcceleration = 15f;      // 공중: 서서히 영향
    [SerializeField] private float airControlMaxSpeed = 8f;    // 공중에서 입력으로 도달 가능한 최대 속도

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

    private Vector3 horizontalVelocity; // 수평 속도 (x, z)
    private float verticalVelocity;     // 수직 속도 (y)
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

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => TryJump();

        inputActions.Player.Sprint.performed += ctx => sprintInput = true;
        inputActions.Player.Sprint.canceled += ctx => sprintInput = false;

        inputActions.Player.Crouch.performed += ctx => crouchInput = true;
        inputActions.Player.Crouch.canceled += ctx => crouchInput = false;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

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

        // 원하는 이동 방향 (입력 기반)
        Vector3 wishDir = transform.right * moveInput.x + transform.forward * moveInput.y;

        float targetSpeed = isCrouching ? crouchSpeed : (sprintInput ? sprintSpeed : walkSpeed);

        if (isGrounded)
        {
            // 지상: 목표 속도로 빠르게 수렴 (기존과 비슷한 반응성)
            Vector3 targetVelocity = wishDir * targetSpeed;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, groundAcceleration * Time.deltaTime);

            // 착지 순간 아래로 살짝 눌러주기 (지형 밀착용, 크지 않게)
            if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }
        else
        {
            // 공중: 기존 관성(horizontalVelocity)은 유지하면서, 입력으로 "가속"만 살짝 가함
            Vector3 addVelocity = wishDir * airAcceleration * Time.deltaTime;
            Vector3 newVelocity = horizontalVelocity + addVelocity;

            // 입력 방향으로의 속도가 airControlMaxSpeed를 넘지 않도록 제한
            // (기존 관성으로 그 이상 빠르면, 그 관성 자체는 안 깎음 — 오버워치 특유의 "빠른 채로 유지"되는 느낌)
            float currentSpeedInWishDir = Vector3.Dot(horizontalVelocity, wishDir);
            if (currentSpeedInWishDir < airControlMaxSpeed)
            {
                horizontalVelocity = newVelocity;
            }
            // wishDir이 0벡터(입력 없음)일 때는 addVelocity도 0이라 자연스럽게 관성만 유지됨
        }

        // 중력 적용
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 fullVelocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(fullVelocity * Time.deltaTime);
    }

    private void TryJump()
    {
        if (controller.isGrounded && !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            // horizontalVelocity는 건드리지 않음 → 점프 시점의 수평 속도가 그대로 공중으로 이어짐
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;

        // 충돌한 대상이 Rigidbody를 가지고 있고, Kinematic이 아닐 때만
        if (hitRigidbody == null || hitRigidbody.isKinematic)
            return;

        // 아래쪽 물체는 밀지 않음 (바닥 등)
        if (hit.moveDirection.y < -0.3f)
            return;

        // 충돌 방향으로 밀어줌
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        hitRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}
