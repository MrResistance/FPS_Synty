using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;

    [SerializeField] private WeaponUI m_weaponUI;
    [SerializeField] private Weapon m_currentWeapon;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #region Event Subscriptions

    private void OnEnable()
    {
        if (WeaponRig.Instance == null)
        {
            return;
        }
        WeaponRig.Instance.UpdateAmmoCounter += SetAmmoCounter;
    }
    private void Start()
    {
        WeaponRig.Instance.UpdateAmmoCounter += SetAmmoCounter;
    }

    private void OnDisable()
    {
        WeaponRig.Instance.UpdateAmmoCounter -= SetAmmoCounter;
    }

    private void OnDestroy()
    {
        WeaponRig.Instance.UpdateAmmoCounter -= SetAmmoCounter;
    }
    #endregion

    public void SetAmmoCounter(int ammoInWeapon, int ammoInReserve)
    {
        m_currentWeapon = WeaponRig.Instance.CurrentWeapon;
        m_weaponUI.SetAmmoCounter(ammoInWeapon, ammoInReserve);
    }


}
