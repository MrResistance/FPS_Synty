using UnityEngine;

public class ForceInitiator : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private float m_upwardForce;
    [SerializeField] private float m_forwardForce;
    private void OnEnable()
    {
        m_rb.AddForce(transform.forward * m_forwardForce, ForceMode.Impulse);
        m_rb.AddForce(transform.up * m_upwardForce, ForceMode.Impulse);
    }
}
