using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Weapon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool m_weaponUnlocked = false;
    [SerializeField] private bool m_hitscan = true;
    public FireMode m_fireMode;
    public enum FireMode { semiAuto, fullAuto }
    public bool WeaponUnlocked => m_weaponUnlocked;
    //public bool Hitscan => m_hitscan;

    [Header("Stats")]
    [SerializeField] private float m_hitForce = 20;
    [SerializeField] private int m_damage = 10;
    [SerializeField] private int m_effectiveRange = 100;

    [Header("Ammo")]
    [SerializeField] private int m_maxClipSize = 30;
    [SerializeField] private int m_currentAmmoInClip;
    [SerializeField] private int m_maxReserveAmmo;
    [SerializeField] private int m_currentReserveAmmo;

    //Public accessors
    public int MaxClipSize => m_maxClipSize;
    public int CurrentAmmoInClip => m_currentAmmoInClip;
    public int CurrentReserveAmmo => m_currentReserveAmmo;

    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_barrel;
    [SerializeField] private ParticleSystem m_gunshotFX;
    [SerializeField] private Transform m_crosshair;

    //Public accessors
    public Animator Animator => m_animator;
    public Transform Barrel => m_barrel;
    public Transform Crosshair => m_crosshair;

    private RaycastHit m_raycastHit;
    private int m_amountToReload;

    #region Event Subscriptions
    private void Start()
    {
        switch (m_fireMode)
        {
            case FireMode.semiAuto:
                ReceivePrimaryPressedEvents();
                break;
            case FireMode.fullAuto:
                ReceivePrimaryHeldEvents();
                break;
        }
        PlayerInputs.Instance.OnReload += Reload;
        m_currentAmmoInClip = m_maxClipSize;
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        switch (m_fireMode)
        {
            case FireMode.semiAuto:
                ReceivePrimaryPressedEvents();
                break;
            case FireMode.fullAuto:
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
    #endregion
    private void FireWeapon()
    {
        if (m_currentAmmoInClip > 0)
        {
            m_currentAmmoInClip--;
            WeaponRig.Instance.UpdateAmmoCounterMethod();
            m_animator.SetTrigger("Fire");
        }
        else
        {
            //OnOutOfAmmo?.Invoke();
        }
    }
    public void Reload()
    {
        int reloadRequestResult = ReloadRequest();
        if (reloadRequestResult > 0)
        {
            m_currentAmmoInClip = 0;
            LoseReserveAmmo(reloadRequestResult);
            m_amountToReload = reloadRequestResult;
            WeaponRig.Instance.UpdateAmmoCounterMethod();
            m_animator.SetTrigger("Reload");
        }
        else
        {
            //OnOutOfAmmoReserve?.Invoke();
        }
    }

    private int ReloadRequest()
    {
        if (m_currentReserveAmmo == 0)
        {
            return 0;
        }
        if (m_currentReserveAmmo > 0 && m_currentReserveAmmo < m_maxClipSize)
        {
            return m_currentReserveAmmo;
        }
        if (m_currentReserveAmmo >= m_maxClipSize)
        {
            return m_maxClipSize;
        }
        return 0;
    }

    public void ReloadComplete()
    {
        m_currentAmmoInClip += m_amountToReload;
        m_amountToReload = 0;
        WeaponRig.Instance.UpdateAmmoCounterMethod();
    }

    public void GainReserveAmmo(int amount)
    {
        m_currentReserveAmmo += amount;
    }

    public void LoseReserveAmmo(int amount)
    {
        m_currentReserveAmmo -= amount;    
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
        // Get the center point of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Create a ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out m_raycastHit, m_effectiveRange))
        {
            if (m_raycastHit.collider.TryGetComponent<Rigidbody>(out _))
            {
                m_raycastHit.rigidbody.AddExplosionForce(m_hitForce, m_raycastHit.point, 1);
            }
            if (m_raycastHit.collider.TryGetComponent(out Damageable damageable))
            {
                damageable.LoseHitPoints(m_damage);
            }
            return m_raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }


}
