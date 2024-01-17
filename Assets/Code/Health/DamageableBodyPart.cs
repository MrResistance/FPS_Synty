using UnityEngine;

public class DamageableBodyPart : MonoBehaviour
{
    [SerializeField] private Damageable m_damageable;
    [SerializeField] private int m_damageModifier = 1;
    public void LoseHitPoints(int damage)
    {
        m_damageable.LoseHitPoints(damage * m_damageModifier);
    }
}
