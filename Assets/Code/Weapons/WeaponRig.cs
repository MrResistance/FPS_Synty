using System.Collections.Generic;
using UnityEngine;

public class WeaponRig : MonoBehaviour
{
    [SerializeField] private Weapon m_currentWeapon;
    [SerializeField] private ParticleSystem m_gunshotFX;

    [SerializeField] private List <Weapon> m_weapons;

    private void Start()
    {
        InitialiseWeapons();
    }

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
}
