using TMPro;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ammoText;
    public void SetAmmoCounter(int ammoInWeapon, int ammoInReserve)
    {
        m_ammoText.text = ammoInWeapon.ToString() + " / " + ammoInReserve.ToString();
    }
}
