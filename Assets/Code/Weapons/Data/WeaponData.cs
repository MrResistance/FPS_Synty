using UnityEngine;
using static Weapon;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "WeaponMenu/NewWeapon", order = 1)]
public class WeaponData : ScriptableObject
{
    public string WeaponName;
    public bool Unlocked = false;

    [Header("Settings")]
    public bool Hitscan = true;
    public Weapon.WeaponType WeaponType;
    public Weapon.FireMode FireMode;

    [Header("Stats")]
    public float HitForce;
    public int Damage;
    public int EffectiveRange;
    public float FireRateCooldown;

    [Header("Ammo")]
    public int MaxClipSize;
    public int CurrentAmmoInClip;
    public int MaxReserveAmmo;
    public int CurrentReserveAmmo;
}
