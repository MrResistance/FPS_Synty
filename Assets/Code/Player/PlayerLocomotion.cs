using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private float m_movementSpeed = 10;
    private Vector3 moveDirection;
    private Vector2 inputVector;
    void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void ProcessInputs()
    {
        inputVector = PlayerInputs.Instance.moveInput;
        moveDirection = transform.right * inputVector.x + transform.forward * inputVector.y;
        moveDirection.Normalize(); // Ensures consistent movement speed in all directions
    }

    void MovePlayer()
    {
        Vector3 targetVelocity = moveDirection * m_movementSpeed;
        // Set the velocity directly without acceleration or deceleration
        m_rb.velocity = new Vector3(targetVelocity.x, m_rb.velocity.y, targetVelocity.z);
    }
}
