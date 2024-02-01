using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData m_weaponData;

    [Header("Settings")]
    public bool WeaponUnlocked;
    [HideInInspector] public WeaponType Type;
    public enum WeaponType { pistol, submachinegun, machinegun, shotgun, explosive, sniper }

    [HideInInspector] public FireMode WeaponFireMode;
    public enum FireMode { semiAuto, fullAuto }

    //Stats
    public float m_hitForce = 20;
    public int m_damage = 10;
    public int m_effectiveRange = 100;
    private float m_fireRateCooldown = 0.1f;

    //Ammo
    private int m_maxClipSize = 30;
    private int m_currentAmmoInClip;
    private int m_maxReserveAmmo;
    private int m_currentReserveAmmo;

    //Public accessors
    public int MaxClipSize => m_maxClipSize;
    public int CurrentAmmoInClip => m_currentAmmoInClip;
    public int CurrentReserveAmmo => m_currentReserveAmmo;

    [Header("Audio")]
    [SerializeField] private List<AudioClip> m_cockWeapon;
    [SerializeField] private List<AudioClip> m_dryFire;
    [SerializeField] private List<AudioClip> m_ejectMag;
    [SerializeField] private List<AudioClip> m_insertMag;
    [SerializeField] private List<AudioClip> m_safetySwitch;
    [SerializeField] private List<AudioClip> m_shot;
    [SerializeField] private List<AudioClip> m_slide;


    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_barrel;
    [SerializeField] private ParticleSystem m_gunshotFX;
    [SerializeField] private Transform m_crosshair;

    //Public accessors
    public Animator Animator => m_animator;
    public Transform Barrel => m_barrel;
    public Transform Crosshair => m_crosshair;

    public RaycastHit m_raycastHit;
    private int m_amountToReload;

    private float m_lastTimeFired;
    protected bool m_currentlyShooting = false;

    public event Action OnShoot;

    #region Event Subscriptions
    protected void Start()
    {
        if (m_weaponData != null)
        {
            GetWeaponData(true);
        }
        else
        {
            Debug.LogWarning(name + " WeaponData not assigned. Weapon Data may be abnormal.");
        }
        
        switch (WeaponFireMode)
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
    protected void OnEnable()
    {
        if (m_weaponData != null)
        {
            GetWeaponData(false);
        }
        else
        {
            Debug.LogWarning(name + " WeaponData not assigned. Weapon Data may be abnormal.");
        }

        if (PlayerInputs.Instance == null) return;
        switch (WeaponFireMode)
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

    protected void OnDisable()
    {
        StopReceivingPrimaryPressedEvents();
        StopReceivingPrimaryHeldEvents();
        PlayerInputs.Instance.OnReload -= Reload;
    }

    protected void OnDestroy()
    {
        StopReceivingPrimaryPressedEvents();
        StopReceivingPrimaryHeldEvents();
        PlayerInputs.Instance.OnReload -= Reload;
    }

    public void ReceivePrimaryPressedEvents()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= RequestShot;
        PlayerInputs.Instance.OnPrimaryPressed += RequestShot;
    }

    public void ReceivePrimaryHeldEvents()
    {
        PlayerInputs.Instance.OnPrimaryHeld -= RequestShot;
        PlayerInputs.Instance.OnPrimaryHeld += RequestShot;
        PlayerInputs.Instance.OnPrimaryReleased -= StopShooting;
        PlayerInputs.Instance.OnPrimaryReleased += StopShooting;
    }

    public void StopReceivingPrimaryPressedEvents()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= RequestShot;
    }

    public void StopReceivingPrimaryHeldEvents()
    {
        PlayerInputs.Instance.OnPrimaryHeld -= RequestShot;
        PlayerInputs.Instance.OnPrimaryReleased -= StopShooting;
    }
    #endregion

    protected void GetWeaponData(bool start)
    {
        Type = m_weaponData.WeaponType;
        WeaponFireMode = m_weaponData.FireMode;
        m_hitForce = m_weaponData.HitForce;
        m_damage = m_weaponData.Damage;
        m_effectiveRange = m_weaponData.EffectiveRange;
        m_fireRateCooldown = m_weaponData.FireRateCooldown;
        m_maxClipSize = m_weaponData.MaxClipSize;
        m_maxReserveAmmo = m_weaponData.MaxReserveAmmo;

        //Audio
        m_cockWeapon = m_weaponData.CockWeapon_SFX;
        m_dryFire = m_weaponData.DryFire_SFX;
        m_ejectMag = m_weaponData.EjectMag_SFX;
        m_insertMag = m_weaponData.InsertMag_SFX;
        m_safetySwitch = m_weaponData.SafetySwitch_SFX;
        m_shot = m_weaponData.Fire_SFX;
        m_slide = m_weaponData.Slide_SFX;


        if (start)
        {
            m_currentAmmoInClip = m_weaponData.CurrentAmmoInClip;
            m_currentReserveAmmo = m_weaponData.CurrentReserveAmmo;
        }

        if (WeaponRig.Instance != null)
        {
            WeaponRig.Instance.UpdateAmmoCounterMethod();
            if (m_cockWeapon.Count > 0)
            {
                WeaponRig.Instance.AudioSource.PlayOneShot(m_cockWeapon[UnityEngine.Random.Range(0, m_cockWeapon.Count)]);
            }
        }
    }
    

    #region Reloading
    public void Reload()
    {
        int reloadRequestResult = RequestReload();
        if (reloadRequestResult > 0)
        {
            if (GameSettings.Instance.RealisticReloadingAmmoCount)
            {
                LoseReserveAmmo(reloadRequestResult);
            }
            else
            {
                LoseReserveAmmo(reloadRequestResult - m_currentAmmoInClip);
            }

            if (m_ejectMag.Count > 0)
            {
                WeaponRig.Instance.AudioSource.PlayOneShot(m_ejectMag[UnityEngine.Random.Range(0, m_ejectMag.Count)]);
            }
            
            m_currentAmmoInClip = 0;
            m_amountToReload = reloadRequestResult;
            m_animator.SetTrigger("Reload");
        }
        else
        {
            //OnOutOfAmmoReserve?.Invoke();
        }
    }

    protected int RequestReload()
    {
        if (m_currentReserveAmmo == 0)
        {
            return 0; //If the player has no reserve ammo, they can't reload
        }
        if (m_currentAmmoInClip == m_maxClipSize)
        {
            return 0; //If the weapon is already fully loaded, they can't reload
        }
        if (m_currentReserveAmmo > 0 && m_currentReserveAmmo < m_maxClipSize)
        {
            return m_currentReserveAmmo; 
            //If the player has reserve ammo, but the reserve ammo is less
            //than the max clip size then return whatever's there in reserve
        }
        if (m_currentReserveAmmo >= m_maxClipSize)
        {
            return m_maxClipSize;
            //If the player has more than or equal to the max clip size, return the max
        }
        return 0;
    }

    public virtual void ReloadComplete()
    {
        m_currentAmmoInClip += m_amountToReload;
        m_amountToReload = 0;

        if (m_insertMag.Count > 0)
        {
            WeaponRig.Instance.AudioSource.PlayOneShot(m_insertMag[UnityEngine.Random.Range(0, m_insertMag.Count)]);
        }
        
        WeaponRig.Instance.UpdateAmmoCounterMethod();
    }
    #endregion
    public void GainReserveAmmo(int amount)
    {
        m_currentReserveAmmo += amount;
    }

    public void LoseReserveAmmo(int amount)
    {
        m_currentReserveAmmo -= amount;    
    }

    #region Shooting
    public virtual void Shoot()
    {
        if (m_currentlyShooting)
        {
            m_currentAmmoInClip--;
            WeaponRig.Instance.UpdateAmmoCounterMethod();

            OnShoot?.Invoke();

            if (m_shot.Count > 0)
            {
                WeaponRig.Instance.AudioSource.PlayOneShot(m_shot[UnityEngine.Random.Range(0, m_shot.Count)]);
            }
            
            m_gunshotFX.Play();
        }
    }
    protected void RequestShot()
    {
        if (Time.time >= m_lastTimeFired + m_fireRateCooldown && !m_currentlyShooting)
        {
            m_lastTimeFired = Time.time;
            if (m_currentAmmoInClip > 0)
            {
                m_currentlyShooting = true;
                m_animator.SetTrigger("Shoot");
            }
            else if (m_dryFire.Count > 0)
            {
                WeaponRig.Instance.AudioSource.PlayOneShot(m_dryFire[UnityEngine.Random.Range(0, m_dryFire.Count)]);
            }
        }
    }
    protected void StopShooting()
    {
        m_currentlyShooting = false;
        m_gunshotFX.Stop();
    }
    #endregion

    public void ResetTrigger(string value)
    {
        m_animator.ResetTrigger(value);
    }
}
