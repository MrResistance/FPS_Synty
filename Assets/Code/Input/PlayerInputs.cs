using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{

    public event Action OnPrimaryPressed;
    public event Action OnSecondaryPressed;
    public event Action OnSecondaryReleased;
    public event Action OnReload;
    public event Action OnJump;
    public event Action<bool> OnSelect;

    public Vector2 moveInput;
    public Vector2 lookInput;

    public PlayerControls controls;

    public static PlayerInputs Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        controls = new PlayerControls();
    }

    #region Enabling/Disabling Action Maps
    private void Start()
    {
        controls.Enable();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnDestroy()
    {
        controls.Disable();
    }
    #endregion

    void Update()
    {
        HandleInputs();
    }

    private void HandleInputs()
    {
        moveInput = controls.Movement.Move.ReadValue<Vector2>();
        lookInput = controls.Movement.Look.ReadValue<Vector2>();

        if (controls.Actions.Primary.phase == InputActionPhase.Performed)
        {
            OnPrimaryPressed?.Invoke();
        }

        if (controls.Actions.Secondary.phase == InputActionPhase.Performed)
        {
            OnSecondaryPressed?.Invoke();
        }
        
        if (!controls.Actions.Secondary.IsPressed())
        { 
            OnSecondaryReleased?.Invoke();
        }

        if (controls.Actions.Reload.ReadValue<float>() > 0)
        {
            OnReload?.Invoke();
        }

        if (controls.Actions.Jump.ReadValue<float>() > 0)
        {
            OnJump?.Invoke();
        }

        if (controls.Actions.Select.ReadValue<float>() > 0)
        {
            OnSelect?.Invoke(true);
        }

        if (controls.Actions.Select.ReadValue<float>() < 0)
        {
            OnSelect?.Invoke(false);
        }

    }
}
