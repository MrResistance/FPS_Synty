using UnityEngine;
using System;

public class AreaOfEffectDamage : MonoBehaviour
{
    private enum areaShape { sphere, box }
    [SerializeField] private areaShape m_areaShape;
    [SerializeField] private float m_explosionRadius;
    [SerializeField] private float m_baseDamage;
    private void OnEnable()
    {
        if (TryGetComponent(out Explosive explosive))
        {
            explosive.OnExplode += DoDamageInArea;
        }
    }

    private void DoDamageInArea()
    {
        switch (m_areaShape)
        {
            case areaShape.sphere:
                var objectsToTakeDamage = Physics.OverlapSphere(transform.position, m_explosionRadius);
                for (int i = 0; i < objectsToTakeDamage.Length; i++)
                {
                    if (objectsToTakeDamage[i].TryGetComponent(out Damageable damageable))
                    {
                        var thisDamage = Convert.ToInt32(m_baseDamage / Vector3.Distance(transform.position, damageable.transform.position));
                        damageable.LoseHitPoints(thisDamage);
                    }
                }
                break;
            case areaShape.box:
                //Physics.OverlapBox(transform.position,);
                break;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_explosionRadius);
    }
}
