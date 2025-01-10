# First Person Interaction Tutorial
> This tutorial will cover how to create a way to interact with objects in the world.

## Prerequisites
Before being able to follow this tutorial you will need to have the correct version of Unity installed and a development IDE. 
This project was created using Unity 2022.3.46f1 and Rider, although you can use any code editor you like.

Additionaly you will need to have a `First Person Controller` to use. You can use mine or create your own. The controller will need to be using the `new input system` as that is how I will be setting up the interactions.

## Installing and Enabling Unitys New Input System
Assuming you have a first person controller with the unitys new input system, you should have already done this. In case you haven't, this is how you install and enable the new input system: Along the top bar click on `Window > Package Manager` and click on `Packges: In Project` to change it to `Packages: Unity Registry`. Then search `Input System` into the search bar and install it into the project. Once it is done installing you will need to enable it in the project. To do this go to `Edit > Project Settings` and then click on `Player`. You should now see a drop down that says `Other Settings` expand this and scroll down until you find `Active Input Handling` and switch the drop down from `Input Manager (old)` to `Input System Handling (new)`. Unity should now restart; If it doesnt, close and reopen the project yourself.

## Setting up the interaction action.

### Setting up the action map
Again, assuming you have a first person controller with the new input system you should have already done this. In case you haven't, this is how you set up an action map: Right click in the `project` and click `Create > Input Actions`. You can now open these `Input Actions` to edit them. Next to where it says `Action Map` press the `+` and name it `Player`. In this map, we can now create every action we want the player to be able to perform.

### Adding the interact action.
Next to `Actions` press the `+` and name the action `Interact`. Press the `+` next to the name and click `Add binding`. Change the `Path` to `E [Keyboard]` or the key you would like to interact with.

## Making the scripts

### Interactable Script

```cs
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
  public virtual void Awake()
  {
    gameObject.layer = 6;
  }

  public abstract void OnInteract();

  public abstract void OnFocus();

  public abstract void OnLoseFocus();
}

```
- This class is going to be the blueprint for all interactable scripts created. Each interactable object type will have its own script which will override these functions. I have decided to do it this way because it may be that different interactable objects need different functionaility. For example, if you interact with a door it will need to open and if you interact with a rock you may need to pick it up.
- The interactable layer is set to 6. So in the inspector you should add a layer called `Interactable` on the 6th layer.

### Interactable Item Script

```cs
using UnityEngine;

public class ItemInteract : Interactable
{
  public override void OnFocus()
  {
    Debug.Log("Looking at object")
  }

  public override void OnLoseFocus()
  {
    Debug.Log("No longer looking at object")
  }

  public override void OnInteract()
  {
    Debug.Log("Interacted with object")
    PickUp();
  }

  private void PickUp()
  {
      Debug.Log("Pick up item")
  }
}
```
- This script is for each item you want to be able to pick up. When the player looks at the object, it should call the `OnFocus` function. When the player stops looking at an object it should call the `OnLoseFocus` function and when the player presses the interact key it should call the `OnInteract` funtion. We still need to set this functionailty up which will be in the next script.

### Editing the First Person Controller Script

I have chosen to put these functions in the first person controller script as that is where I think they belong.
First you need to create this function and call it in `Update`.

**Variables.**

```cs
[Header("Interaction")]
[SerializeField] private Vector3 interactionRayPoint;
[SerializeField] private float interactionDistance;
[SerializeField] private LayerMask interactionLayer;
private Interactable _currentInteractable;
```
- First I added a `Header` for ease of use so it shows up seperately to the other `variables` in the `inspector`.
- The first variable is the where the `raycast` is shot out from. I set it to `0.5f, 0.5f` as that is the center of the viewport.
- The second variable is how `far` the `raycast` shoots. I set this to `2` but you can tweak to your liking.
- The third variable is the `layer` that gets `interacted` with. This is set to `Interactable` in slot 6.
- The fourth layer is used to `store` an `object` that is currently being `focused` on.
![image](https://github.com/user-attachments/assets/7bb0abd5-c444-4fb1-9845-b2ad123ebfc7)

**InteractionCheck function.**

```cs
private void HandleInteractionCheck()
{
  if(Physics.Raycast(_playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
  {
    if (hit.collider.gameObject.layer != 6 || (_currentInteractable &&
                                               hit.collider.gameObject.GetInstanceID() ==
                                               _currentInteractable.GetInstanceID())) return;
      
      hit.collider.TryGetComponent(out _currentInteractable);
          
      if(_currentInteractable)
          _currentInteractable.OnFocus();
  }
  else if (_currentInteractable)
  {
      _currentInteractable.OnLoseFocus();
      _currentInteractable = null;
  }
}
```
- First a `Raycast` is show out from the player at the `interactionRayPoint` , which is set in the inspector (Default is the center of the screen `0.5f, 0.5f`), at the `interactionDistance`.
- If the `raycast` hits something and the `layer` of the object is `not` the `interactable` later or there is a `_currentInteractable` and the object being looked at is the same as the `_currentInteractable` then just return. This is because I onlt want the `OnFocus()` function to be called once when you initially focus on the interactable.
- If the `raycast` hits something and the previous conditions are not met then it attempts to get the `interactable` component of the `_currentInteractable`. If the comoponent is not found, then `_currentInteractable` remains null, else it calls the `OnFocus()` function on the `interactable`.
- If the `raycast` does not hit anything and there was a previous interactable in `_currentInteractable`: Calls the `OnLoseFocus()` function on that `interactable` and sets the `_currentInteractable` to `null` since the player is no longer looking at it.

**InteractionInput function.**

```cs
public void HandleInteractionInput(InputAction.CallbackContext context)
{
  if(_currentInteractable && Physics.Raycast(_playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
  {
    _currentInteractable.OnInteract();
  }
}
```
- This function is called whenever the `interaction` key is pressed. It won't work yet, you will also need to follow the next step as well.
- If there is a `_currentInteractable` being looked at and the raycast is still hitting a valid object then call the `OnInteract` function on the object.
- The reason we check with another raycast is just incase the player looks away and `_currentInteractable` doesn't update or update in time and the player is able to interact with the object while not looking at it. 
