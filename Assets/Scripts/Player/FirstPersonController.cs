using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Input")]
    [HideInInspector] public Vector2 mouseVector;
    private PlayerInputActions _playerInputActions;
    
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    private bool _isSprinting;
    private CharacterController _characterController;
    private Vector3 _moveDirection;

    [Header("Look Parameters")]
    [SerializeField, Range(1,180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1,180)] private float lowerLookLimit = 80.0f;
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;
    private Camera _playerCamera;
    private float _rotationX = 0;
    
    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;
    
    public static FirstPersonController Instance;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    private void Awake()
    {
        if (Instance !=  null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        _playerCamera = Camera.main;  
        _characterController = GetComponent<CharacterController>();  

        _playerInputActions = PlayerInputManager.Instance.PlayerInputActions; 
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();

        ApplyFinalMovements();
    }

    private void HandleMovement()
    {
        var inputVector = _playerInputActions.Player.Movement.ReadValue<Vector2>(); // Reads the value of the player movement input

        // set currentInput variable to vector 2 sprint/walk speed * vertical axis, sprint/walk speed * horizontal axis. Which speed depends on if the user is holding the sprint key or not.
        float speed = _isSprinting ? sprintSpeed : walkSpeed;

        var moveDirectionY = _moveDirection.y;  // Create a float variable of the move direction of the users y coordinates
        
        _moveDirection = (transform.forward * inputVector.y * speed) + (transform.right * inputVector.x * speed);
        _moveDirection.y = moveDirectionY;
    }

    public void HandleSprint(InputAction.CallbackContext context)
    {
        if (context.performed) _isSprinting = true; // If sprint key held, sprint
        if (context.canceled) _isSprinting = false; // If sprint key let go, don't sprint
    }
    
    public void HandleJump(InputAction.CallbackContext context)
    {
        // Raises the player a specified amount into the air if they are grounded
        if(_characterController.isGrounded)
            _moveDirection.y = jumpForce; 
    }
    
    private void HandleMouseLook()
    {
        mouseVector = _playerInputActions.Player.Look.ReadValue<Vector2>(); // Reads player mouse / joystick look
        
        _rotationX -= mouseVector.y * verticalSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -upperLookLimit, lowerLookLimit);
        
        _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseVector.x * horizontalSpeed, 0);
    }
    
    private void ApplyFinalMovements()
    {   
        if (!_characterController.isGrounded)
        {
            // Starts moving the character down if he is in the air
            _moveDirection.y -= gravity * Time.deltaTime;

            // If player hits the ceiling
            if(_characterController.collisionFlags == CollisionFlags.Above)
            {
                // Stop adding velocity to the character
                _moveDirection = Vector3.zero;
                // This prevents the player from sticking
                // to the ceiling and moving along it
                _characterController.stepOffset = 0;
            }
        }
        // When the player is actually grounded
        else
        {
            if(_characterController.stepOffset == 0)
            {
                // Resets the stepOffset to the default float so
                // the character is still able to function correctly
                // while walking up steps or across bumps.
                _characterController.stepOffset = 0.3f;
            }
        }

        _characterController.Move(_moveDirection * Time.deltaTime);
    }
}