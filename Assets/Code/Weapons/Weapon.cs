using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private float m_hitForce = 20;
    [SerializeField] private int m_damage = 10;

    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private ParticleSystem m_muzzleFlashFX;
    [SerializeField] private GunshotRenderer m_gunshotRenderer;
    private RaycastHit m_raycastHit;
    public Animator Animator => m_animator;
    public ParticleSystem MuzzleFlashFX => m_muzzleFlashFX;
    #region Event Subscriptions
    private void Start()
    {
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }

    private void OnDisable()
    {
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
    public void PlayWeaponFX()
    {
        MuzzleFlashFX?.Play();
        m_gunshotRenderer.UpdateLinePositions(HitCalculation());
    }
    public void ResetTrigger()
    {
        m_animator.ResetTrigger("Fire");
        m_gunshotRenderer.ClearLine();
    }

    private Vector3 HitCalculation()
    {
        if (Physics.Raycast(m_gunshotRenderer.transform.position, m_gunshotRenderer.transform.forward, out m_raycastHit))
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
