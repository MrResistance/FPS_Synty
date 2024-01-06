using System;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private float m_movementSpeed = 10;
    [SerializeField] private float m_maxMovementSpeed = 50;
    [SerializeField] private float stopDamping = 5f; // Damping factor when input is zero

    void Start()
    {
        PlayerInputs.Instance.OnMove += Move;
        PlayerInputs.Instance.OnStopMove += StopMove;
    }

    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnMove -= Move;
        PlayerInputs.Instance.OnMove += Move;
        PlayerInputs.Instance.OnStopMove -= StopMove;
        PlayerInputs.Instance.OnStopMove += StopMove;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnMove -= Move;
        PlayerInputs.Instance.OnStopMove -= StopMove;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnMove -= Move;
        PlayerInputs.Instance.OnStopMove -= StopMove;
    }
    private void Move(Vector2 vector)
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = (forward * vector.y + right * vector.x).normalized;

        // Scale the force based on current speed to prevent exceeding max speed
        float speedFactor = (m_maxMovementSpeed - m_rb.velocity.magnitude) / m_maxMovementSpeed;
        speedFactor = Mathf.Clamp01(speedFactor);

        m_rb.AddForce(desiredMoveDirection * m_movementSpeed * speedFactor * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void StopMove()
    {
        // Apply damping when there is no input
        if (m_rb.velocity.magnitude > 0)
        {
            m_rb.velocity -= m_rb.velocity * stopDamping * Time.deltaTime;
        }
    }
}
