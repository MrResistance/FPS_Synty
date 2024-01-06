using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_actionAsset;
    [SerializeField] private InputActionMap m_movementMap;
    [SerializeField] private InputActionMap m_actionsMap;

    [SerializeField] private InputActionProperty m_move;
    [SerializeField] private InputActionProperty m_look;
    [SerializeField] private InputActionProperty m_primary;
    [SerializeField] private InputActionProperty m_secondary;
    [SerializeField] private InputActionProperty m_reload;
    [SerializeField] private InputActionProperty m_jump;

    public event Action<Vector2> OnMove;
    public event Action OnStopMove;
    public event Action<Vector2> OnLook;
    public event Action OnPrimaryPressed;
    public event Action OnSecondaryPressed;
    public event Action OnReload;
    public event Action OnJump;

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
        m_movementMap = m_actionAsset.FindActionMap("Movement");
        m_actionsMap = m_actionAsset.FindActionMap("Actions");
    }

    #region Enabling/Disabling Action Maps
    private void Start()
    {
        m_movementMap.Enable();
        m_actionsMap.Enable();
    }

    private void OnEnable()
    {
        m_movementMap.Enable();
        m_actionsMap.Enable();
    }

    private void OnDisable()
    {
        m_movementMap.Disable();
        m_actionsMap.Disable();
    }

    private void OnDestroy()
    {
        m_movementMap.Disable();
        m_actionsMap.Disable();
    }
    #endregion

    void Update()
    {
        HandleInputs();
        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        OnLook?.Invoke(m_look.action.ReadValue<Vector2>());
    }

    private void HandleInputs()
    {
        Vector2 moveInput = m_move.action.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
        {
            OnMove?.Invoke(moveInput);
        }

        if (moveInput == Vector2.zero)
        {
            OnStopMove?.Invoke();
        }

        if (m_primary.action.phase == InputActionPhase.Performed)
        {
            OnPrimaryPressed?.Invoke();
        }

        if (m_secondary.action.ReadValue<float>() > 0)
        {
            OnSecondaryPressed?.Invoke();
        }

        if (m_reload.action.ReadValue<float>() > 0)
        {
            OnReload?.Invoke();
        }

        if (m_jump.action.ReadValue<float>() > 0)
        {
            OnJump?.Invoke();
        }
    }
}
