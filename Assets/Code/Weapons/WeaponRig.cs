using System.Collections.Generic;
using UnityEngine;

public class WeaponRig : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Weapon m_currentWeapon;
    [SerializeField] private ParticleSystem m_gunshotFX;

    [SerializeField] private Camera m_camera;
    [SerializeField] private float m_ADS_FOV = 30;
    [SerializeField] private float m_HipFire_FOV = 60;
    [SerializeField] private float m_transitionDuration = 1.0f; // Duration of the transition in seconds
    private float m_transitionProgress = 0f; // Progress of the transition
    private bool m_isAimingDownSight = false;

    [SerializeField] private Vector3 m_aimDownSightPosition;
    [SerializeField] private Vector3 m_aimFromHipPosition;
    [SerializeField] private List <Weapon> m_weapons;
    [SerializeField] private int m_currentWeaponLocation;

    #region Event Subscriptions
    private void Start()
    {
        InitialiseWeapons();
        PlayerInputs.Instance.OnSecondaryPressed += AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased += AimFromHip;
        PlayerInputs.Instance.OnSelect += SelectWeapon;
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnSecondaryPressed -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryPressed += AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
        PlayerInputs.Instance.OnSecondaryReleased += AimFromHip;
        PlayerInputs.Instance.OnSelect -= SelectWeapon;
        PlayerInputs.Instance.OnSelect += SelectWeapon;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnSecondaryPressed -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
        PlayerInputs.Instance.OnSelect -= SelectWeapon;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnSecondaryPressed -= AimDownSight;
        PlayerInputs.Instance.OnSecondaryReleased -= AimFromHip;
        PlayerInputs.Instance.OnSelect -= SelectWeapon;
    }
    #endregion

    private void InitialiseWeapons()
    {
        for (int i = 0; i < transform.childCount; i++) 
        {
            if (transform.GetChild(i).TryGetComponent(out Weapon weapon))
            {
                m_weapons.Add(weapon);
                weapon.gameObject.SetActive(false);
            }
        }

        if (m_weapons.Count > 0)
        {
            m_currentWeapon = m_weapons[0];
            m_currentWeapon.gameObject.SetActive(true);
            SetGunshotFX_Parent();
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
        m_currentWeapon.enabled = true;
        m_currentWeapon.gameObject.SetActive(true);
        SetGunshotFX_Parent();
    }


    private void SetGunshotFX_Parent()
    {
        m_gunshotFX?.transform.SetParent(m_currentWeapon.barrel.transform);
        m_gunshotFX?.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        m_currentWeapon.SetGunshotFX(m_gunshotFX);
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

    private void TransitionView()
    {
        if (m_transitionProgress < 1.0f)
        {
            m_transitionProgress += Time.deltaTime / m_transitionDuration;

            if (m_isAimingDownSight)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_aimDownSightPosition, m_transitionProgress);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, m_transitionProgress);
                m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_ADS_FOV, m_transitionProgress);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_aimFromHipPosition, m_transitionProgress);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, m_transitionProgress);
                m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_HipFire_FOV, m_transitionProgress);
            }
        }
    }

    private void Update()
    {
        TransitionView();
    }

}
