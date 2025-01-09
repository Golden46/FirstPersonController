# First Person Controller Tutorial
> This tutorial will cover how to create a first person controller where you can move, look around, jump, and sprint.

## Prerequisites
Before being able to follow this tutorial you will need to have the correct version of Unity installed and a development IDE. 
This project was created using Unity 2022.3.46f1 and Rider, although you can use any code editor you like.

Additionaly you will need knowledge of Unity's New Input System.

## Setting up the new input system.

### Installing and enabling
In order to use the new input system you will have to do a few things first. Along the top bar click on `Window > Package Manager` and click on `Packges: In Project` to change it to `Packages: Unity Registry`. Then search `Input System` into the search bar and install it into the project. Once it is done installing you will need to enable it in the project. To do this go to `Edit > Project Settings` and then click on `Player`. You should now see a drop down that says `Other Settings` expand this and scroll down until you find `Active Input Handling` and switch the drop down from `Input Manager (old)` to `Input System Handling (new)`. Unity should now restart; If it doesnt, close and reopen the project yourself.

### Setting up the Player Input Actions
> In order for the input to work we need to set up an action map with the actions we want.

**Set up the input actions.**

Right click in the `project` and click `Create > Input Actions`. You can now open these `Input Actions` to edit them. Next to where it says `Action Map` press the `+` and name it `Player`. In these map, we can now create every action we want the player to be able to perform.

<br>

**Set up the Movement action.**

Next to `Actions` press the `+` and name the first action `Movement`. Change the `Action Type` to `Value` and `Control Type` to `Vector 2` because we are dealing with `WASD` movement. Next, press the `+` next to `Movement` and click `Add Up/Down/Left/Right Composite`. Now, click on `Up` and set the `Path` to `W [Keyboard]` and do the same for `Down (S)`, `Left (A)`, and `Right (D)`. 

<br>

**Set up the Sprint action.**

Next to `Actions` press the `+` and name the action `Sprint`. Press the `+` next to the name and click `Add binding`. Change the `Path` to `Left Shift [Keyboard]` or the key you would like to sprint with.

<br>

**Set up the Jump action.**

Next to `Actions` press the `+` and name the action `Jump`. Press the `+` next to the name and click `Add binding`. Change the `Path` to `Jump [Keyboard]` or the key you would like to jump with.

<br>

**Set up the Look action.**

Next to `Actions` press the `+` and name the action `Look`. Change the `Action Type` to `Value` and `Control Type` to `Delta` because we are dealing with mouse movement as opposed to keyboard presses. Press the `+` next to the name and click `Add binding`. Change the `Path` to `Delta [Mouse]`

<br>

**This is what it should look like.**

![image](https://github.com/user-attachments/assets/c87536a3-87ca-4c9d-b9ce-e4520b13e7b7)

## Setting up the Player Input Manager
> Now the actions are set up, we need a way to use them.

> [!IMPORTANT]
> First, click on the `Input Actions` and look at the `Inspector` view. There should be a checkbox that says `Generate C# Class`; Click on the checkbox and press apply. This will now allow us to use these actions.

### Player Input Manager Variables.

```cs
public PlayerInputActions PlayerInputActions;
private FirstPersonController _fpc;
public static PlayerInputManager Instance;
```
- The first variable is the input actions we just generated.
- The second variable is a reference to the `FirstPersonController` script.
- The third variable is to make the class a `singleton`.

> [!NOTE]
> Simply put a `singleton` is a globally accessible class that can only exist once in the scene.

<br>

### Player Input Manager Awake Function.

```cs
private void Awake()
{
  if (Instance !=  null && Instance != this) Destroy(this);
  else Instance = this;

  PlayerInputActions ??= new PlayerInputActions();
}
```
- The first two lines will `destroy` the script if an `instance` already exists in the scene or set the variable `Instance` to the sript.
- The last line creates a new `PlayerInputActions` if it is curently `null`. the `??=` operator is called a `Null-Coalescing Assignment Operator`.

<br>

### Player Input Manager OnEnable Function

```cs
private void OnEnable()
{
  _fpc = FirstPersonController.Instance ?? FindObjectOfType<FirstPersonController>();
      
  PlayerInputActions.Player.Enable();
  PlayerInputActions.Player.Jump.performed += _fpc.HandleJump;
  PlayerInputActions.Player.Sprint.performed += _fpc.HandleSprint;
  PlayerInputActions.Player.Sprint.canceled += _fpc.HandleSprint;
}
```
- `_fpc` is set to the left-hand assignment if it is `not null` otherwise it is set to the right-hand assignment. Similar to before this is called a `Null-Coalescing Operator`
- Next, with the second line, we have to `enable` the player `action map` in order to use it.
- The last 3 lines are what allow us to use the different `actions`. These can be commented out until we actually make those functions in the `FirstPersonController` script.

> [!NOTE]
> The `+=` operator here is used to `subscribe` a `method` to an `event`. For example the `HandleJump` method or function is being subscribed to the `performed` event. The `performed` event is called when the action is done and the `canceled` event is called when the action is stopped. So when the `Jump` key is performed it will activate the `HandleJump` method.

<br>

### Player Input Manager OnDisable Function

```cs
private void OnDisable()
{
  PlayerInputActions.Player.Jump.performed -= _fpc.HandleJump;
  PlayerInputActions.Player.Sprint.performed -= _fpc.HandleSprint;
  PlayerInputActions.Player.Sprint.canceled -= _fpc.HandleSprint;
  PlayerInputActions.Player.Disable();
}
```
- When the script is `disabled` we need to `unsubscribe` from all of the events and then `disable` the player `action map`.

## Setting up the First Person Controller
> Now we have a way to use the actions, we can create the controller.

### First Person Controller Variables.

```cs
[Header("Input")]
[HideInInspector] public Vector2 mouseVector;
private PlayerInputActions _playerInputActions;
```
- First variable is used to read the `movement vector` input.
- Second variable is to get a reference to the `player actions`.

<br>

```cs
[Header("Movement Parameters")]
[SerializeField] private float walkSpeed = 5.0f;
[SerializeField] private float sprintSpeed = 8.0f;
private bool _isSprinting;
private CharacterController _characterController;
private Vector3 _moveDirection;
```
- First and second variable is the walk and sprint `speed`.
- Third variable is used to decide if the player is sprinting or not.
- Fourth variable is a reference to the `character controller` on the player.
- Fifth variable is the `direction` the player is moving in.

<br>

```cs
[Header("Look Parameters")]
[SerializeField, Range(1,180)] private float upperLookLimit = 80.0f;
[SerializeField, Range(1,180)] private float lowerLookLimit = 80.0f;
[SerializeField] private float horizontalSpeed = 10f;
[SerializeField] private float verticalSpeed = 10f;
private Camera _playerCamera;
private float _rotationX = 0;
```
- First and second variables are used to `clamp` the camera when `looking` up and down.
- Third and fourth variables are the `speeds` in which the camera `rotates`.
- Fifth variable is a `reference` to the camera.
- Sixth variable will be used to store the x rotation of the camera.

<br>

```cs
[Header("Jumping Parameters")]
[SerializeField] private float jumpForce = 8.0f;
[SerializeField] private float gravity = 30.0f;

public static FirstPersonController Instance;
```
- First variable is the `force` in which the `player jumps` up.
- Second variable is the `force` in which the `player falls` back down.
- Third variable is used to make this class into a `singleton`.

<br>

### First Person Controller OnEnable Function.

```cs
private void OnEnable()
{
  Cursor.lockState = CursorLockMode.Locked;
  Cursor.visible = false; 
}
```
- This locks the cursor to the center of the screen and hides it.

<br>

### First Person Controller Awake Function.

```cs
private void Awake()
{
  if (Instance !=  null && Instance != this) Destroy(gameObject);
  else Instance = this;
}
```
- This will `destroy` the script if an `instance` already exists in the scene or set the variable `Instance` to the sript.

<br>

### First Person Controller Start Function.

```cs
private void Start()
{
  _playerCamera = Camera.main;  
  _characterController = GetComponent<CharacterController>();  

  _playerInputActions = PlayerInputManager.Instance.PlayerInputActions; 
}
```
- First gets a reference to the main `camera` which is on the player.
- Then gets a reference to the `character controller` component on the player.
- Then gets a reference to the `Player Input Actions` we created earlier.

<br>

### First Person Controller Update Function.

```cs
private void Update()
{
  HandleMovement();
  HandleMouseLook();

  ApplyFinalMovements();
}
```
- This function calls the main `movement` functions every frame. 

<br>

### First Person Controller Movement.

```cs
private void HandleMovement()
{
  var inputVector = _playerInputActions.Player.Movement.ReadValue<Vector2>(); 
  
  float speed = _isSprinting ? sprintSpeed : walkSpeed;
  
  var moveDirectionY = _moveDirection.y;
  
  _moveDirection = (transform.forward * inputVector.y * speed) + (transform.right * inputVector.x * speed);
  _moveDirection.y = moveDirectionY;
}
```
- First we read the input vector from the `Movement` action.
- Then we set the `move speed` to the `walk` or `sprint` speed based on whether the `_isSprinting` bool is true or false. This will be set up later.
- Then we keep the `y move direction` the same because that is not affected by the `WASD` movement.
- We then get the move direction by performing a simple calculation using the `speed` and `inputVector`.

<br>

```cs
private void HandleMouseLook()
{
  mouseVector = _playerInputActions.Player.Look.ReadValue<Vector2>();
  
  _rotationX -= mouseVector.y * verticalSpeed;
  _rotationX = Mathf.Clamp(_rotationX, -upperLookLimit, lowerLookLimit);
  
  _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
  transform.rotation *= Quaternion.Euler(0, mouseVector.x * horizontalSpeed, 0);
}
```
- First we read the `mouse vector` from the `Look` action.
- Then we calculate the `up` and `down` movement by getting the `vertical` mouse movement and `multiplying` it by a `speed` value.
- Then we clamp how far `up` and `down` the camera can actually move based on the variables we set up earlier.
- After that, we rotate the camera by the amount we just calculated.
- To rotate `left` and `right` we using the `horizontal` mouse movement and rotate the actual player with a `speed` `multiplier`.

<br>

```cs
private void ApplyFinalMovements()
{   
  if (!_characterController.isGrounded)
  {
    _moveDirection.y -= gravity * Time.deltaTime;

    if(_characterController.collisionFlags == CollisionFlags.Above)
    {
      _moveDirection = Vector3.zero;
      _characterController.stepOffset = 0;
    }
  }
  else
  {
    if(_characterController.stepOffset == 0) _characterController.stepOffset = 0.3f;
  }

  _characterController.Move(_moveDirection * Time.deltaTime);
}
```
- First, if the player is `not grounded`, we move the player down by the `gravity` amount specified earlier.
- Also, if the character is in the `air` and `hits` and object above them, their upwards movement gets cancelled and they fall back down to the ground. If we didn't do this, the player would stick to the ceiling for a bit before falling back down.
- Their `step offset` is also set to 0 to prevent any unwanted movement while colliding with any object above them.
- If the player is `grounded` and they have just hit a `ceiling`, their `step offset` gets `reset` back to `0.3f` which is the `default` value.
- Finally, the `character controller` moves the player in the `_moveDirection` calculated in `HandleMovement`.

<br>

### Player Controller Extra Movement.
```cs
public void HandleSprint(InputAction.CallbackContext context)
{
  if (context.performed) _isSprinting = true;
  if (context.canceled) _isSprinting = false;
}
```
- This function is called every time the `player` `presses` and `lets go` of the sprint key.
- If it is `pressed` the `_isSprinting` bool is set to true.
- If it is `let go` of the `_isSprinting` bool is set to false.

<br>

```cs
public void HandleJump(InputAction.CallbackContext context)
  {
    if(_characterController.isGrounded)
        _moveDirection.y = jumpForce; 
  }
```
- This function is called every time the `jump` key is pressed.
- If the `character` is on the `ground` then the players y movement is set to the `jumpForce` which propells them upwards.

<br>

## Setting up the scene
> Now the programming is all done, we just need to set up the scene.

### Environment.

First, spawn in a `3D plane` and scale it up for a floor; Then spawn in a few `3D Cubes` and place them around the floor so you are able to see whether you are moving or not.

<img width="208" alt="Screenshot 2025-01-07 at 08 40 56" src="https://github.com/user-attachments/assets/44b59b0a-7d91-47fc-920c-36ad70b5b0d5" />

### Player.

Then spawn a `3D Capsule` which is going to be the player. Right click the `transform` and click `reset`. Click on the `Main Camera` and also `reset` the `transform` then make it a `child` of the `3D Capsule`. Set the `Y position` of the `player` to a number which puts it fully `above` the `floor` such as `1.1` then set the `Y position` of the camera to `1` so it is at head height. Now add the `Character Controller` component to the capsule and also the `First Person Controller` script.

![image](https://github.com/user-attachments/assets/430311c0-4965-4771-96ea-7e0aac444cbf)

### The end.

Lastly create an `Empty Game Object` and add the `Player Input Manager` script onto it.

![image](https://github.com/user-attachments/assets/2c5361d4-eeed-46a5-a1f3-e07a980014af)
