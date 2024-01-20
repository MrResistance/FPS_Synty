using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private Zombie m_zombie;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damageable damageable))
        {
            m_zombie.objectsToDamage.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent(out Damageable damageable);
        if (m_zombie.objectsToDamage.Contains(damageable))
        {
            m_zombie.objectsToDamage.Remove(damageable);
        }
    }
}
