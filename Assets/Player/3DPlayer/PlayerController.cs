using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

/// <summary>
/// Controls the character by magnetizing to nearby objects that he can walk on.
/// </summary>
[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public const float DEFAULT_MOVEMENT_SPEED = 5f;
    public const float DEFAULT_SPRINT_SPEED = 8f;
    public const float DEFAULT_IN_AIR_SPEED = 15f;
    public const float DEFAULT_ROTATION_SPEED = 100f;
    public const float DEFAULT_FOV= 60f;
    public const float DEFAULT_SPRINT_FOV= 65f;
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
    public bool isMageticBootsOn = false;

    /// <summary>
    /// Is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            //Debug.LogError($"Missing reference at {nameof(animator)}.");

        rigidbody.useGravity = false;
        rigidbody.angularDrag = DEFAULT_RIGIDBODY_ANGULAR_DRAG;
    }

    /// <summary>
    /// Is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isGrounded = true;
        currentMagneticRotationSpeed = magneticRotationSpeedGrounded;
    }

    /// <summary>
    /// Is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    /// 
    private void FixedUpdate()
    {
        Move();
        ProcessLook();
        if (isMageticBootsOn) { MagneticRotate(); }
        Gravitate();
    }

    private void Update()
    {
        Jump();
        JudgeSprint();
        if (cam.fieldOfView != fov) { DoFieldofView();}
        if (Input.GetKeyDown(KeyCode.Q)) { isMageticBootsOn = !isMageticBootsOn;UpdateUI(); }
    }

    public void UpdateUI()
    {
        playerToUI.MagneticBootsUI(isMageticBootsOn.ToString());
    }

    /// <summary>
    /// Moves the character using user's input.
    /// </summary>
    private void Move()
    {
        // Forward and backward movement.
        var inputVertical = Input.GetAxis("Vertical");
        var inputHorizontal = Input.GetAxis("Horizontal");
        if (inputHorizontal == 0 && inputVertical == 0 && inAir == false && isGrounded == true)
        {
            inMovement = false;
            inSprint = false;
            ResetMovement();
        }
        else
        {
            inMovement = true;
        }
        animator?.SetFloat("WalkingSpeed", inputVertical);
        var verticalPositionOffset = transform.forward * inputVertical * movementSpeed;
        var hortizontalPositionOffset = transform.right * inputHorizontal * movementSpeed;
        var positionOffset = verticalPositionOffset + hortizontalPositionOffset;
        var targetPosition = transform.position + positionOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime);
    }

    public void InAir(bool value)
    {
        if (value)
        {
            inAir = true;
            isGrounded = false;
            movementSpeed = DEFAULT_IN_AIR_SPEED;
        }
        else {
            inAir = false;
            movementSpeed = DEFAULT_MOVEMENT_SPEED;
            rigidbody.velocity = Vector3.zero;
        }
    }
    private void JudgeSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && inMovement && !inSprint)
        {
            inSprint = true;
            movementSpeed = DEFAULT_SPRINT_SPEED;
            fov = DEFAULT_SPRINT_FOV;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && inMovement && inSprint)
        {
            inSprint = false;
            ResetMovement();
        }
    }

    private void ResetMovement()
    {
        movementSpeed = DEFAULT_MOVEMENT_SPEED;
        fov = DEFAULT_FOV;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidbody.AddForce(transform.up * jumpForce);
            isGrounded = false;
            currentMagneticRotationSpeed = magneticRotationSpeedFly;
        }
    }

    public void ProcessLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;

        // Calculate camera rotation for looking up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Smoothly interpolate the camera rotation
        Vector3 targetCamRotation = new Vector3(xRotation, 0f, 0f);
        currentCamRotation = Vector3.SmoothDamp(currentCamRotation, targetCamRotation, ref camRotationVelocity, rotationSmoothTime);

        cam.transform.localRotation = Quaternion.Euler(currentCamRotation);

        // Smoothly interpolate the player rotation
        float targetPlayerRotation = transform.eulerAngles.y + mouseX;
        currentPlayerRotation = Mathf.SmoothDampAngle(currentPlayerRotation, targetPlayerRotation, ref playerRotationVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0f, currentPlayerRotation, 0f);
    }

    public void DoFieldofView()
    {
        float currFov = cam.fieldOfView;
        cam.fieldOfView = Mathf.Lerp(currFov, fov, Time.deltaTime * 10);
        if (currFov.ToString("F4") == fov.ToString("F4"))
        {
            cam.fieldOfView = fov;
        }
    }

    /// <summary>
    /// Rotates the character using user's input.
    /// </summary>
    private void Rotate()
    {
        // Left and right turn in place.
        var inputHorizontal = Input.GetAxis("Horizontal");
        animator?.SetFloat("RotatingSpeed", inputHorizontal);
        var rotationOffset = Quaternion.AngleAxis(inputHorizontal * rotationSpeed, Vector3.up);
        var targetRotation = transform.rotation * rotationOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime);
    }

    /// <summary>
    /// Rotates the character to the nearest walking surface.
    /// </summary>
    private void MagneticRotate()
    {
        var stickedRotation = GetMagneticRotation();
        transform.rotation = Quaternion.Slerp(transform.rotation, stickedRotation, Time.fixedDeltaTime * currentMagneticRotationSpeed);
    }

    /// <summary>
    /// Gets the target rotation quaternion to the nearest walking surface.
    /// Shoots three SphereCasts: forward, down and back. Than selects the nearest one from their hits, and returns the created rotation to the hit's normal.
    /// </summary>
    /// <returns>The target rotation quaternion.</returns>
    private Quaternion GetMagneticRotation()
    {
        closestSphereCastHit = null;

        if (Physics.SphereCast(transform.position, collider.radius, -transform.up + transform.forward, out RaycastHit hitForward, SPHERECAST_DISTANCE, RayCastLayerMask))
            closestSphereCastHit = hitForward;

        if (Physics.SphereCast(transform.position, collider.radius, -transform.up, out RaycastHit hitDown, SPHERECAST_DISTANCE, RayCastLayerMask))
        {
            if (!closestSphereCastHit.HasValue || closestSphereCastHit.Value.distance > hitDown.distance)
                closestSphereCastHit = hitDown;
        }

        if (Physics.SphereCast(transform.position, collider.radius, -transform.up - transform.forward, out RaycastHit hitBack, SPHERECAST_DISTANCE, RayCastLayerMask))
        {
            if (!closestSphereCastHit.HasValue || closestSphereCastHit.Value.distance > hitBack.distance)
                closestSphereCastHit = hitBack;
        }

        var normal = new Vector3(0, 0, 0);
        normal = closestSphereCastHit?.normal ?? Vector3.zero;

        if (normal.sqrMagnitude > float.Epsilon)
        {
            return Quaternion.LookRotation(Vector3.Cross(transform.right, normal), normal);
        }
        else
        {
            var stickedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 playerUp = transform.rotation * Vector3.up;
            float dotProduct = Vector3.Dot(playerUp, Vector3.up);
            if (dotProduct < 0)
            {
                stickedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 180);
            }
            return stickedRotation;
        }
    }

    /// <summary>
    /// Moves the character by the gravity force.
    /// </summary>
    private void Gravitate()
    {
        if (isMageticBootsOn)
        {
            rigidbody.useGravity = false;
            rigidbody.AddForce(-transform.up * Time.deltaTime * gravitySpeed);
        }
        else
        {
            rigidbody.useGravity = true;
            NormalGravitateRotate();
        }
    }

    private void NormalGravitateRotate()
    {
        var stickedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        Vector3 playerUp = transform.rotation * Vector3.up;
        float dotProduct = Vector3.Dot(playerUp, Vector3.up);
        if (dotProduct < 0)
        {
            stickedRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 180);
        }
        if (transform.rotation.eulerAngles.x < 1){ transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);}
        if (transform.rotation.eulerAngles.z < 1) { transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0); }
        transform.rotation = Quaternion.Slerp(transform.rotation, stickedRotation, Time.fixedDeltaTime * currentMagneticRotationSpeed);
    }

    /// <summary>
    /// Is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collided">The another collided object.</param>
    private void OnCollisionEnter(Collision collided)
    {
        // Checks if the character is collide with objects on which he can walk.
        if (((1 << collided.gameObject.layer) & RayCastLayerMask) == 0)
            return;

        animator?.SetBool("IsJumping", false);
        isGrounded = true;
        if (inAir){InAir(false); }
        currentMagneticRotationSpeed = magneticRotationSpeedGrounded;

        // Stick to animated platform.
        if (collided.gameObject.tag == "StickyPlatform")
            transform.SetParent(collided.transform);
    }

    /// <summary>
    /// Is called when this collider/rigidbody has stopped touching another rigidbody/collider.
    /// </summary>
    /// <param name="collided">The another collided object.</param>
    private void OnCollisionExit(Collision collided)
    {
        // Unstick to animated platform.
        if (collided.gameObject.tag == "StickyPlatform")
            transform.SetParent(null);
    }
}