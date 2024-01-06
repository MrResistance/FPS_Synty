using UnityEngine;

public class WeaponRig : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;

    #region Event Subscriptions
    private void Start()
    {
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }
    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
        PlayerInputs.Instance.OnPrimaryPressed += FireWeapon;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnPrimaryPressed -= FireWeapon;
    }
    #endregion

    private void FireWeapon()
    {
        currentWeapon.Animator.SetTrigger("Fire");
    }

    private void Update()
    {
           
    }
}
