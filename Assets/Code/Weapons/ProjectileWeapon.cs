using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [SerializeField] private string objectPoolerTag;
    [SerializeField] private Transform m_barrel;

    public virtual void SpawnProjectile()
    {
        ObjectPooler.Instance.SpawnFromPool(objectPoolerTag, m_barrel.position, m_barrel.rotation);
    }
}
