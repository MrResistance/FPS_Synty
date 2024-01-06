using System;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private float m_movementSpeed = 10;
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
        // Get the camera's forward and right vectors, but ignore the y component to keep movement horizontal.
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;

        // Normalize these vectors to ensure they have a magnitude of 1.
        forward.Normalize();
        right.Normalize();

        // Calculate the desired move direction relative to the camera's orientation.
        Vector3 desiredMoveDirection = (forward * vector.y + right * vector.x) * m_movementSpeed;

        // Apply the force to the Rigidbody.
        m_rb.AddForce(desiredMoveDirection * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void StopMove()
    {
        //m_rb.velocity = new Vector3(0, m_rb.velocity.y, 0);
    }
}
