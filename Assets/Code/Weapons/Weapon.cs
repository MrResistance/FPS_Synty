using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private ParticleSystem m_muzzleFlashFX;
    [SerializeField] private Transform m_bulletTrailFX_Position;
    [SerializeField] private GunshotRenderer m_gunshotRenderer;
    public Animator Animator => m_animator;
    public ParticleSystem MuzzleFlashFX => m_muzzleFlashFX;
    public Transform BulletTrailFX_Position => m_bulletTrailFX_Position;
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
        m_gunshotRenderer.UpdateLinePositions(GetHitPosition());
    }
    public void ResetTrigger()
    {
        m_animator.ResetTrigger("Fire");
        m_gunshotRenderer.ClearLine();
    }

    private Vector3 GetHitPosition()
    {
        if (Physics.Raycast(m_gunshotRenderer.transform.position, m_gunshotRenderer.transform.forward, out RaycastHit raycastHit))
        {
            return raycastHit.point;
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
