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
    protected float m_hitForce = 20;
    protected int m_damage = 10;
    protected int m_effectiveRange = 100;
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

    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_barrel;
    [SerializeField] private ParticleSystem m_gunshotFX;
    [SerializeField] private Transform m_crosshair;

    //Public accessors
    public Animator Animator => m_animator;
    public Transform Barrel => m_barrel;
    public Transform Crosshair => m_crosshair;

    protected RaycastHit m_raycastHit;
    private int m_amountToReload;

    private float m_lastTimeFired;
    protected bool m_currentlyFiring = false;

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
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }

    public void ReceivePrimaryHeldEvents()
    {
        PlayerInputs.Instance.OnPrimaryHeld -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryHeld += FireWeapon;
        PlayerInputs.Instance.OnPrimaryReleased -= StopPlayingWeaponFX;
        PlayerInputs.Instance.OnPrimaryReleased += StopPlayingWeaponFX;
    }

    public void StopReceivingPrimaryPressedEvents()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
    }

    public void StopReceivingPrimaryHeldEvents()
    {
        PlayerInputs.Instance.OnPrimaryHeld -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryReleased -= StopPlayingWeaponFX;
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

        if (start)
        {
            m_currentAmmoInClip = m_weaponData.CurrentAmmoInClip;
            m_currentReserveAmmo = m_weaponData.CurrentReserveAmmo;
        }

        if (WeaponRig.Instance != null)
        {
            WeaponRig.Instance.UpdateAmmoCounterMethod();
        }
    }
    protected void FireWeapon()
    {
        if (Time.time >= m_lastTimeFired + m_fireRateCooldown)
        {
            m_lastTimeFired = Time.time;
            if (m_currentAmmoInClip > 0)
            {
                m_currentAmmoInClip--;
                WeaponRig.Instance.UpdateAmmoCounterMethod();
                m_currentlyFiring = true;
                m_animator.SetTrigger("Fire");
            }
            else
            {
                StopPlayingWeaponFX();
                //OnOutOfAmmo?.Invoke();
            }
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
            m_animator.SetTrigger("Reload");
        }
        else
        {
            //OnOutOfAmmoReserve?.Invoke();
        }
    }

    protected int ReloadRequest()
    {
        if (m_currentReserveAmmo == 0)
        {
            return 0;
        }
        if (m_currentAmmoInClip == m_maxClipSize)
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

    public virtual void ReloadComplete()
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

    public virtual void PlayWeaponFX()
    {
        if (m_currentlyFiring)
        {
            m_gunshotFX.Play();
        }
    }

    protected void StopPlayingWeaponFX()
    {
        m_currentlyFiring = false;
        m_gunshotFX.Stop();
    }

    public void ResetTrigger(string value)
    {
        m_animator.ResetTrigger(value);
    }
}
