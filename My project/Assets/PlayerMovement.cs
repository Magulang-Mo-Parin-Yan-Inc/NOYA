using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterStats stats;
    public StaminaSystem staminaSystem;

    public float baseMoveSpeed = 6f;
    public float sprintSpeedMultiplier = 1.8f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    public Transform cameraTransform;

    public float jumpStaminaCost = 15f;
    public float sprintStaminaCostPerSecond = 20f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private float turnSmoothVelocity;

    // Sprint detection
    private float lastWTapTime = 0f;
    private float doubleTapThreshold = 0.3f;
    private bool isSprinting = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleSprintInput();

        float currentSpeed = baseMoveSpeed;
        float frameStaminaCost = sprintStaminaCostPerSecond * Time.deltaTime;

        if (isSprinting)
        {
            if (stats.currentStamina >= frameStaminaCost)
            {
                currentSpeed *= sprintSpeedMultiplier;
                staminaSystem.UseStamina(frameStaminaCost);
            }
            else
            {
                isSprinting = false;
                Debug.Log("Stopped sprinting - stamina too low");
            }
        }

        // Ground check and reset vertical velocity if grounded
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Handle animation blending
        float speedPercent = direction.magnitude;

        if (isSprinting && speedPercent > 0.1f)
        {
            speedPercent = 1f; // force full run
        }
		if (!isSprinting && speedPercent > 0.1f)
        {
            speedPercent = .5f;
        }
        if (animator != null)
        {
            animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        }

        // Move character
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            if (staminaSystem.UseStamina(jumpStaminaCost))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (animator != null)
                {
                    animator.SetBool("isJumping", true);
                }
            }
        }

        if (controller.isGrounded && animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", false);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleSprintInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWTapTime < doubleTapThreshold)
            {
                if (stats.currentStamina > 0)
                {
                    isSprinting = true;
                    Debug.Log("Started sprinting");
                }
            }
            lastWTapTime = Time.time;
        }

        if (!Input.GetKey(KeyCode.W) && isSprinting)
        {
            isSprinting = false;
            Debug.Log("Stopped sprinting - W released");
        }
    }
}
