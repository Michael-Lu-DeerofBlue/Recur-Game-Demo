using Kamgam.SettingsGenerator;
using Unity.VisualScripting;
using UnityEngine;
using Fungus;

/// <summary>
/// Controls the character by magnetizing to nearby objects that he can walk on.
/// </summary>
[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class WToMoveUp : MonoBehaviour
{
    public const float DEFAULT_MOVEMENT_SPEED = 5f;
    public const float DEFAULT_SPRINT_SPEED = 8f;
    public const float DEFAULT_IN_AIR_SPEED = 15f;
    public const float DEFAULT_ROTATION_SPEED = 100f;
    public const float DEFAULT_FOV = 60f;
    public const float DEFAULT_SPRINT_FOV = 65f;
    public const float DEFAULT_MAGNETIC_ROTATION_SPEED_GROUNDED = 10f;
    public const float DEFAULT_MAGNETIC_ROTATION_SPEED_FLYING = 0.5f;
    public const float DEFAULT_GRAVITY_SPEED = 500f;
    public const float DEFAULT_JUMP_FORCE = 500f;
    public const float SPHERECAST_DISTANCE = 5f;
    public const float DEFAULT_RIGIDBODY_ANGULAR_DRAG = 100f;
    public const int LAYER_EVERYTHING = 0;

    [SerializeField] public float movementSpeed = DEFAULT_MOVEMENT_SPEED;
    [SerializeField] private float rotationSpeed = DEFAULT_ROTATION_SPEED;
    [SerializeField] private float magneticRotationSpeedGrounded = DEFAULT_MAGNETIC_ROTATION_SPEED_GROUNDED;
    [SerializeField] private float magneticRotationSpeedFly = DEFAULT_MAGNETIC_ROTATION_SPEED_FLYING;
    [SerializeField] private float gravitySpeed = DEFAULT_GRAVITY_SPEED;
    [SerializeField] private float jumpForce = DEFAULT_JUMP_FORCE;
    [SerializeField] private float fov = DEFAULT_FOV;

    [Header("Layers on which the character can walk.")]
    [SerializeField] private LayerMask RayCastLayerMask = ~LAYER_EVERYTHING;

    private float currentMagneticRotationSpeed;
    public bool isGrounded;
    public bool inMovement;
    public bool inSprint;
    public bool inAir;
    public new Rigidbody rigidbody;
    public PlayerToUI playerToUI;
    private new CapsuleCollider collider;
    private Animator animator;
    private RaycastHit? closestSphereCastHit;
    public float checkRadius = 0.5f; // Define the check radius
    public LayerMask obstacleLayer;  // Define the layer of obstacles
    private float previousYPosition;
    private bool isJumping = false;

    [Header("Camera Look")]
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 30f; public float ySensitivity = 30f;
    public float rotationSmoothTime = 0.1f;
    private Vector3 currentCamRotation;
    private Vector3 camRotationVelocity;
    private float currentPlayerRotation;
    private float playerRotationVelocity;

    [Header("Magnetic Boots")]
    public bool isMagneticBootsOn = false;

    public Flowchart flowchart;

    /// <summary>
    /// Is called when the script instance is being loaded.
    /// </summary>
   
    private void FixedUpdate()
    {
        Move();
        ProcessLook();
      
    }

    private void Move()
    {
        // Forward and backward movement.
        var inputVertical = Input.GetAxis("Vertical");
        var inputHorizontal = Input.GetAxis("Horizontal");
        animator?.SetFloat("WalkingSpeed", inputVertical);
        // Calculate position offsets based on input
        var upPositionOffset = transform.up * inputVertical * movementSpeed;
        var horizontalPositionOffset = transform.right * inputHorizontal * movementSpeed;
        var positionOffset = upPositionOffset + horizontalPositionOffset;
        var targetPosition = transform.position + positionOffset;

        // Perform raycast to check for obstacles

        RaycastHit hit;
        bool isObstacleAhead = Physics.Raycast(transform.position, positionOffset.normalized, out hit, checkRadius, obstacleLayer);
        if (!isObstacleAhead)
        {
            // No obstacle ahead, proceed with the movement
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime);
        }
        else
        {
            // Obstacle detected, you can handle it here if needed
            Debug.Log("Obstacle detected ahead: " + hit.collider.name);
        }
    }

    public void ProcessLook()
    {

        float mouseX = Input.GetAxis("Mouse X") * xSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity;

        // Calculate camera rotation for looking up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Smoothly interpolate the camera rotation
        Vector3 targetCamRotation = new Vector3(xRotation, 0f, 0f);
        currentCamRotation = Vector3.SmoothDamp(currentCamRotation, targetCamRotation, ref camRotationVelocity, rotationSmoothTime);

        cam.transform.localRotation = Quaternion.Euler(currentCamRotation);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
        /*
        // Smoothly interpolate the player rotation
        float targetPlayerRotation = transform.eulerAngles.y + mouseX;
        currentPlayerRotation = Mathf.SmoothDampAngle(currentPlayerRotation, targetPlayerRotation, ref playerRotationVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0f, currentPlayerRotation, 0f);
        */
    }
}