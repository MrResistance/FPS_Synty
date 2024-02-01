using UnityEngine;

public class RocketProjectileWeapon : ProjectileWeapon
{
    [SerializeField] private GameObject m_visualRocket;
    public override void SpawnProjectile()
    {
        base.SpawnProjectile();
        m_visualRocket.SetActive(false);
    }

    public void ReloadComplete()
    {
        m_visualRocket.SetActive(true);
    }
}
