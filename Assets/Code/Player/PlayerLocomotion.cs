using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private float m_movementSpeed = 10;
    void Start()
    {
        PlayerInputs.Instance.OnMove += Move;
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnMove -= Move;
        PlayerInputs.Instance.OnMove += Move;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnMove -= Move;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnMove -= Move;
    }
    private void Move(Vector2 vector)
    {
        m_rb.AddForce(new Vector3(vector.x, 0, vector.y) * m_movementSpeed * Time.deltaTime);
    }
}
