using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRig : MonoBehaviour
{
    public static WeaponRig Instance;

    [Header("References")]
    [SerializeField] private Weapon m_currentWeapon;
    public Weapon CurrentWeapon => m_currentWeapon;

    [Header("Camera Settings")]
    [SerializeField] private Camera m_camera;
    [SerializeField] private Camera m_sniperScopeCamera;
    [SerializeField] private float m_SniperScope_FOV = 5;
    [SerializeField] private float m_ADS_Sniper_FOV = 10;
    [SerializeField] private float m_ADS_FOV = 30;
    [SerializeField] private float m_HipFire_FOV = 60;
    [SerializeField] private float m_transitionDuration = 1.0f; // Duration of the transition in seconds
    private float m_transitionProgress = 0f; // Progress of the transition
    private bool m_isAimingDownSight = false;

    [SerializeField] private Vector3 m_aimDownSightPosition;
    [SerializeField] private Vector3 m_aimFromHipPosition;

    [Header("Current Weapons List")]
    [SerializeField] private List<Weapon> m_weapons;
    [SerializeField] private int m_currentWeaponLocation;

    public event Action<int, int> UpdateAmmoCounter;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        #endregion
    }

    #region Event Subscriptions
    private void Start()
    {
        PlayerInputs.Instance.OnSecondaryHeld += AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased += AimFromHip;
        PlayerInputs.Instance.OnSelect += SelectWeapon;
        InitialiseWeapons();
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnSecondaryHeld -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryHeld += AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
        PlayerInputs.Instance.OnSecondaryReleased += AimFromHip;
        PlayerInputs.Instance.OnSelect -= SelectWeapon;
        PlayerInputs.Instance.OnSelect += SelectWeapon;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnSecondaryHeld -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
        PlayerInputs.Instance.OnSelect -= SelectWeapon;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnSecondaryHeld -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
        PlayerInputs.Instance.OnSelect -= SelectWeapon;
    }
    #endregion

    private void InitialiseWeapons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Weapon weapon) && weapon.WeaponUnlocked)
            {
                m_weapons.Add(weapon);
                weapon.gameObject.SetActive(false);
            }
        }

        if (m_weapons.Count > 0)
        {
            m_currentWeapon = m_weapons[0];
            CurrentWeaponSetup();
        }
    }

    private void SelectWeapon(bool upOrDown)
    {
        if (m_weapons.Count <= 0)
        {
            Debug.LogWarning("No weapons available to select.");
            return;
        }

        m_currentWeaponLocation = m_weapons.IndexOf(m_currentWeapon);
        m_currentWeapon.enabled = false;
        m_currentWeapon.gameObject.SetActive(false);

        if (upOrDown)
        {
            // Move to the next weapon, wrapping around to the start if at the end
            m_currentWeaponLocation++;
            if (m_currentWeaponLocation == m_weapons.Count)
            {
                m_currentWeaponLocation = 0;
            }
        }
        else
        {
            // Move to the previous weapon, wrapping around to the end if at the start
            m_currentWeaponLocation--;
            if (m_currentWeaponLocation < 0)
            {
                m_currentWeaponLocation = m_weapons.Count - 1;
            }
        }

        m_currentWeapon = m_weapons[m_currentWeaponLocation];
        CurrentWeaponSetup();
    }

    private void CurrentWeaponSetup()
    {
        m_currentWeapon.enabled = true;
        m_currentWeapon.gameObject.SetActive(true);

        switch (m_currentWeapon.Type)
        {
            case Weapon.WeaponType.sniper:
                m_sniperScopeCamera = m_currentWeapon.gameObject.GetComponent<Sniper>().SniperScopeCamera;
                CrosshairManager.Instance.DisableCrosshairs();
                break;
            default:
                CrosshairManager.Instance.SetCrosshair(m_currentWeapon.Crosshair);
                break;
        }

        UpdateAmmoCounterMethod();
    }

    private void AimDownSight()
    {
        m_isAimingDownSight = true;
        m_transitionProgress = 0f; // Reset progress
    }

    private void AimFromHip()
    {
        m_isAimingDownSight = false;
        m_transitionProgress = 0f; // Reset progress
    }
    private void Update()
    {
        TransitionView();
    }
    private void TransitionView()
    {
        if (m_transitionProgress < 1.0f)
        {
            m_transitionProgress += Time.deltaTime / m_transitionDuration;

            if (m_isAimingDownSight)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_aimDownSightPosition, m_transitionProgress);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, m_transitionProgress);
                if (m_currentWeapon.Type == Weapon.WeaponType.sniper)
                {
                    m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_ADS_Sniper_FOV, m_transitionProgress);
                    m_sniperScopeCamera.fieldOfView = Mathf.Lerp(m_sniperScopeCamera.fieldOfView, m_SniperScope_FOV, m_transitionProgress);
                }
                else
                {
                    m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_ADS_FOV, m_transitionProgress);
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_aimFromHipPosition, m_transitionProgress);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, m_transitionProgress);
                m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_HipFire_FOV, m_transitionProgress);
            }
        }
    }

    public void UpdateAmmoCounterMethod()
    {
        if (m_currentWeapon != null)
        {
            UpdateAmmoCounter?.Invoke(m_currentWeapon.CurrentAmmoInClip, m_currentWeapon.CurrentReserveAmmo);
        }
    }
}
