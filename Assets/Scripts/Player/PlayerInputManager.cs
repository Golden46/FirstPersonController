using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerInputActions PlayerInputActions;
    private FirstPersonController _fpc;
    public static PlayerInputManager Instance;
    
    private void Awake()
    {
        if (Instance !=  null && Instance != this) Destroy(this);
        else Instance = this;
        
        DontDestroyOnLoad(this);
        
        PlayerInputActions ??= new PlayerInputActions();
    }

    private void OnEnable()
    {
        _fpc = FirstPersonController.Instance ?? FindObjectOfType<FirstPersonController>();
            
        PlayerInputActions.Player.Enable();
        PlayerInputActions.Player.Jump.performed += _fpc.HandleJump;
        PlayerInputActions.Player.Sprint.performed += _fpc.HandleSprint;
        PlayerInputActions.Player.Sprint.canceled += _fpc.HandleSprint;
    }
    
    private void OnDisable()
    {
        PlayerInputActions.Player.Disable();
        PlayerInputActions.Player.Jump.performed -= _fpc.HandleJump;
        PlayerInputActions.Player.Sprint.performed -= _fpc.HandleSprint;
        PlayerInputActions.Player.Sprint.canceled -= _fpc.HandleSprint;

    }
}