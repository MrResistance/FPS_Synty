using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private LayerMask m_groundMask;
    [SerializeField] private Transform m_camera;
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private float m_movementSpeed = 10;
    [SerializeField] private float m_jumpForce = 20;
    private Vector3 m_moveDirection;
    private Vector2 m_inputVector;

    private void Start()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnJump += Jump;
    }

    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnJump -= Jump;
        PlayerInputs.Instance.OnJump += Jump;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnJump -= Jump;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnJump -= Jump;
    }

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
        m_inputVector = PlayerInputs.Instance.moveInput;
        m_moveDirection = transform.right * m_inputVector.x + transform.forward * m_inputVector.y;
        m_moveDirection.Normalize(); // Ensures consistent movement speed in all directions
    }

    void MovePlayer()
    {
        Vector3 targetVelocity = m_moveDirection * m_movementSpeed;
        // Set the velocity directly without acceleration or deceleration
        m_rb.velocity = new Vector3(targetVelocity.x, m_rb.velocity.y, targetVelocity.z);
    }

    void Jump()
    {
        if (GroundCheck())
        {
            m_rb.AddForce(transform.up * m_jumpForce, ForceMode.Impulse);
        }
    }

    bool GroundCheck()
    {
        var objectsAroundPlayer = Physics.OverlapSphere(transform.position, 0.25f);
        for (int i = 0; i < objectsAroundPlayer.Length; i++)
        {
            if (((1 << objectsAroundPlayer[i].gameObject.layer) & m_groundMask) != 0)
            {
                return true;
            }
        }
        return false;
    }
}
