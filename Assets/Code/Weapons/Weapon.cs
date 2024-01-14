using UnityEngine;

[SelectionBase]
public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData m_weaponData;

    [Header("Settings")]
    [SerializeField] private bool m_weaponUnlocked = false;
    [SerializeField] private bool m_hitscan = true;
    public WeaponType Type;
    public enum WeaponType { pistol, submachinegun, machinegun, shotgun, explosive, sniper }

    public FireMode WeaponFireMode;
    public enum FireMode { semiAuto, fullAuto }
    public bool WeaponUnlocked => m_weaponUnlocked;

    [Header("Stats")]
    [SerializeField] private float m_hitForce = 20;
    [SerializeField] private int m_damage = 10;
    [SerializeField] private int m_effectiveRange = 100;
    [SerializeField] private float m_fireRateCooldown = 0.1f;

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

    private float m_lastTimeFired;
    private bool m_currentlyFiring = false;

    #region Event Subscriptions
    private void Start()
    {
        if (m_weaponData != null)
        {
            GetWeaponData();
        }
        else
        {
            Debug.LogWarning("WeaponData not assigned. Weapon Data may be abnormal.");
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
        m_currentAmmoInClip = m_maxClipSize;
    }
    private void OnEnable()
    {
        if (m_weaponData != null)
        {
            GetWeaponData();
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

    private void GetWeaponData()
    {
        m_weaponUnlocked = m_weaponData.Unlocked;
        m_hitscan = m_weaponData.Hitscan;
        Type = m_weaponData.WeaponType;
        WeaponFireMode = m_weaponData.FireMode;
        m_hitForce = m_weaponData.HitForce;
        m_damage = m_weaponData.Damage;
        m_effectiveRange = m_weaponData.EffectiveRange;
        m_fireRateCooldown = m_weaponData.FireRateCooldown;
        m_maxClipSize = m_weaponData.MaxClipSize;
        m_currentAmmoInClip = m_weaponData.CurrentAmmoInClip;
        m_maxReserveAmmo = m_weaponData.MaxReserveAmmo;
        m_currentReserveAmmo = m_weaponData.CurrentReserveAmmo;
    }
    private void FireWeapon()
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

    public void PlayWeaponFX()
    {
        Debug.Log("Play Weapon FX");
        if (m_currentlyFiring)
        {
            m_gunshotFX.Play();
            if (m_hitscan)
            {
                HitCalculation();
            }
        }
    }

    private void StopPlayingWeaponFX()
    {
        Debug.Log("Stop Firing Weapon");
        m_currentlyFiring = false;
        m_gunshotFX.Stop();
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
