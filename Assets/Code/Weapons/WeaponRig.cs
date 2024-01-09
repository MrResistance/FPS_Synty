using System.Collections.Generic;
using UnityEngine;

public class WeaponRig : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Weapon m_currentWeapon;
    [SerializeField] private ParticleSystem m_gunshotFX;
    [SerializeField] private Camera m_camera;

    [SerializeField] private Vector3 m_aimDownSightPosition;
    [SerializeField] private Vector3 m_aimFromHipPosition;
    [SerializeField] private List <Weapon> m_weapons;
    
    [SerializeField] private float m_ADS_FOV = 30;
    [SerializeField] private float m_HipFire_FOV = 60;
    #region Event Subscriptions
    private void Start()
    {
        InitialiseWeapons();
        PlayerInputs.Instance.OnSecondaryPressed += AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased += AimFromHip;
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnSecondaryPressed -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryPressed += AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
        PlayerInputs.Instance.OnSecondaryReleased += AimFromHip;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnSecondaryPressed -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnSecondaryPressed -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
    }
    #endregion

    private void InitialiseWeapons()
    {
        for (int i = 0; i < transform.childCount; i++) 
        {
            if (transform.GetChild(i).TryGetComponent(out Weapon weapon))
            {
                m_weapons.Add(weapon);
            }
        }

        if (m_weapons.Count > 0)
        {
            m_currentWeapon = m_weapons[0];
            SetGunshotFX_Parent();
        }
    }

    private void SelectWeapon(bool upOrDown)
    {
        int currentWeaponLocation = m_weapons.IndexOf(m_currentWeapon);

        if (upOrDown)
        {
            m_currentWeapon = m_weapons[currentWeaponLocation + 1];
        }
        else
        {
            m_currentWeapon = m_weapons[currentWeaponLocation - 1];
        }

        SetGunshotFX_Parent();
    }

    private void SetGunshotFX_Parent()
    {
        m_gunshotFX.transform.SetParent(m_currentWeapon.barrel.transform);
        m_gunshotFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        m_currentWeapon.SetGunshotFX(m_gunshotFX);
    }

    private void AimDownSight()
    {
        transform.SetLocalPositionAndRotation(m_aimDownSightPosition ,Quaternion.identity);
        m_camera.fieldOfView = m_ADS_FOV;
    }
    private void AimFromHip()
    {
        transform.SetLocalPositionAndRotation(m_aimFromHipPosition, Quaternion.identity);
        m_camera.fieldOfView = m_HipFire_FOV;
    }
}
