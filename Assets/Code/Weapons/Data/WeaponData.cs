using UnityEngine;
using static Weapon;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon Menu/New Weapon", order = 1)]
public class WeaponData : ScriptableObject
{
    public string WeaponName;

    [Header("Settings")]
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
