# First Person Controller and Interactable Learning Journal
This log is a record of my progress with the first person controller tutorial and interactable tutorial.


## 2024-11-22

### Null error
- When I was testing the `Player Input Manager` script, the jump and sprint was not working. I soon realised this was becaause the `_fpc` variable was null at runtime. At first I was confused as it had worked beforehand so I did a little research. At first I was setting `_fpc` to `FirstPersonController.Instance`; What I found was if the `Player Input Manager` start and awake functions call before the `First Person Controller` ones, it can cause the `_fpc` variable in `Player Input Manager` to be null. I believe it was referred to as `race conditions`. This is why I did it this way `_fpc = FirstPersonController.Instance ?? FindObjectOfType<FirstPersonController>();` using the `Null-Coalescing Operator`. `_fpc` is set to the left-hand assignment if it is not null otherwise it is set to the right-hand assignment. This fixed the issue. I could have just set `_fpc` to `FindObjectOfType<FirstPersonController>();` but I thought it was nice to have a fallback in any case.

## 2024-11-25

### Mouse rotation
- When I was working out how to do the mouse rotation, I really struggled to get the movement working properly. This is because I was rotating the camera when looking up and down but rotating the player when looking left and right. So when I looked left and held the `W` key, the player would no longer move forward but move to the side. It took me a while to figure out but I found this solution to calculate the move direction: `_moveDirection = (transform.forward * inputVector.y * speed) + (transform.right * inputVector.x * speed);`. So now, no matter what way the play is looking, the movement keys will move you in the expected direction.

## 2024-12-12

### User Error with Interactions.
- The only issue I had when making this interaction system was `user error` on my part. When testing the `interactions`, I initially forgot to set the `InteractionRayPoint` in the inspector so it was set to `0, 0`. This means that the `raycast` wasn't shooting out from the correct place on the player. I resolved this by remembering I had to set it to `0.5f, 0.5f` to have the `raycast` shoot out from the center of the screen. Next, at one point I put the `Interactable` layer on a different layer that wasnt `number 6` so the interactions were not working there and I also kept forgetting to assign the `Interactable` layer to the objects.

Other than these issues, the making of the controller and interaction system went quite well.
