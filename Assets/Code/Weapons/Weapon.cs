using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private float m_hitForce = 20;
    [SerializeField] private int m_damage = 10;
    [SerializeField] private Transform m_barrel;
    [HideInInspector] public Transform barrel => m_barrel;
    [SerializeField] private ParticleSystem m_gunshotFX;

    [Header("References")]
    [SerializeField] private Animator m_animator;
    private RaycastHit m_raycastHit;
    public Animator Animator => m_animator;
    #region Event Subscriptions
    private void Start()
    {
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }
    private void OnEnable()
    {
        Debug.Log(name + " enabled.");
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }

    private void OnDisable()
    {
        Debug.Log(name + " disabled.");
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
    }
    #endregion

    private void FireWeapon()
    {
        m_animator.SetTrigger("Fire");
    }

    public void SetGunshotFX(ParticleSystem gunshotFX)
    {
        m_gunshotFX = gunshotFX;
    }

    public void PlayWeaponFX()
    {
        m_gunshotFX.Play();
        HitCalculation();
    }

    public void ResetTrigger()
    {
        m_animator.ResetTrigger("Fire");
    }

    private Vector3 HitCalculation()
    {
        if (Physics.Raycast(m_barrel.transform.position, m_barrel.transform.forward, out m_raycastHit))
        {
            if (m_raycastHit.collider.TryGetComponent(out Rigidbody rb))
            {
                m_raycastHit.rigidbody.AddExplosionForce(m_hitForce, m_raycastHit.point, 1);
            }
            return m_raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
