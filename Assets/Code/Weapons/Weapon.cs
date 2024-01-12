using System.Collections;
using UnityEngine;

[SelectionBase]
public class Weapon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool m_weaponUnlocked = false;
    [SerializeField] private bool m_hitscan = true;
    public fireMode m_fireMode;
    public enum fireMode { semiAuto, fullAuto }
    public bool WeaponUnlocked => m_weaponUnlocked;
    public bool Hitscan => m_hitscan;

    [Header("Stats")]
    [SerializeField] private float m_hitForce = 20;
    [SerializeField] private int m_damage = 10;
    [SerializeField] private int m_effectiveRange = 100;

    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_barrel;
    [HideInInspector] public Transform barrel => m_barrel;
    [SerializeField] private ParticleSystem m_gunshotFX;
    private RaycastHit m_raycastHit;
    public Animator Animator => m_animator;
    #region Event Subscriptions
    private void Start()
    {
        switch (m_fireMode)
        {
            case fireMode.semiAuto:
                ReceivePrimaryPressedEvents();
                break;
            case fireMode.fullAuto:
                ReceivePrimaryHeldEvents();
                break;
        }
        PlayerInputs.Instance.OnReload += Reload;
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        switch (m_fireMode)
        {
            case fireMode.semiAuto:
                ReceivePrimaryPressedEvents();
                break;
            case fireMode.fullAuto:
                ReceivePrimaryHeldEvents();
                break;
        }
        PlayerInputs.Instance.OnReload -= Reload;
        PlayerInputs.Instance.OnReload += Reload;
    }

    private void OnDisable()
    {
        StopReceivingPrimaryPressedEvents();
        StopReceivingPrimaryHeldEvents();
        PlayerInputs.Instance.OnReload -= Reload;
    }

    private void OnDestroy()
    {
        StopReceivingPrimaryPressedEvents();
        StopReceivingPrimaryHeldEvents();
        PlayerInputs.Instance.OnReload -= Reload;
    }
    #endregion

    public void ReceivePrimaryPressedEvents()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }

    public void ReceivePrimaryHeldEvents()
    {
        PlayerInputs.Instance.OnPrimaryHeld -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryHeld += FireWeapon;
    }

    public void StopReceivingPrimaryPressedEvents()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
    }

    public void StopReceivingPrimaryHeldEvents()
    {
        PlayerInputs.Instance.OnPrimaryHeld -= FireWeapon;
    }
    private void FireWeapon()
    {
        m_animator.SetTrigger("Fire");
    }
    public void Reload()
    {
        m_animator.SetTrigger("Reload");
    }

    public void SetGunshotFX(ParticleSystem gunshotFX)
    {
        m_gunshotFX = gunshotFX;
    }

    public void PlayWeaponFX()
    {
        m_gunshotFX?.Play();
        if (m_hitscan)
        {
            HitCalculation();
        }
    }

    public void ResetTrigger(string value)
    {
        m_animator.ResetTrigger(value);
    }

    private Vector3 HitCalculation()
    {
        if (Physics.Raycast(m_barrel.transform.position, m_barrel.transform.forward, out m_raycastHit, m_effectiveRange))
        {
            Debug.Log("Hit: " + m_raycastHit.collider.name);
            if (m_raycastHit.collider.TryGetComponent<Rigidbody>(out _))
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
