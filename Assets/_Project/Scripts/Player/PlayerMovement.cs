using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Transform cameraTransform = null; // Cinemachine Follow target
    [SerializeField] private Transform playerModel = null; // the mesh or model to rotate
    [SerializeField] private Animator animator;


    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSmooth = 10f; // how fast the player turns

    private Vector3 velocity;
    private Vector2 inputVector;
    private bool movementEnabled = true;
    private bool sprinting = false;

    private void Reset()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main ? Camera.main.transform : null;
        if (!playerModel) playerModel = transform; // default to root
        if (!animator) animator = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        ApplyGravity();
        ApplyMovement();
        ApplyRotation();

        controller.Move(velocity * Time.deltaTime);

        //calculate speed
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;

        float blendSpeed = horizontalSpeed / sprintSpeed;
        blendSpeed = Mathf.Clamp01(blendSpeed);

        animator.SetFloat("Speed", blendSpeed);


    }

    public void Move(Vector2 input)
    {
        inputVector = input;
    }

    private void ApplyMovement()
    {
        if (!movementEnabled) return;

        Vector3 forward = cameraTransform ? Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
        Vector3 right = cameraTransform ? cameraTransform.right : Vector3.right;

        Vector3 desired = (forward * inputVector.y + right * inputVector.x).normalized;
        float targetSpeed = sprinting ? sprintSpeed : walkSpeed;
        float horizontalSpeed = Mathf.Lerp(new Vector2(velocity.x, velocity.z).magnitude, targetSpeed, Time.deltaTime * acceleration);

        Vector3 horizontalVelocity = desired * horizontalSpeed;
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
    }

    public void Jump()
    {
        if (!movementEnabled) return;
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");

        }
    }

    public void SetSprinting(bool value)
    {
        sprinting = value;
    }

    public void EnableMovement(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled)
            velocity = Vector3.zero;
    }

    public Vector3 GetMovementDirection()
    {
        Vector3 forward = cameraTransform ? Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
        Vector3 right = cameraTransform ? cameraTransform.right : Vector3.right;
        return (forward * inputVector.y + right * inputVector.x).normalized;
    }

    private void ApplyRotation()
    {
        Vector3 moveDir = GetMovementDirection();
        if (moveDir.sqrMagnitude > 0.01f) // only rotate if moving
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            playerModel.rotation = Quaternion.Lerp(playerModel.rotation, targetRotation, rotationSmooth * Time.deltaTime);
        }
    }
}
