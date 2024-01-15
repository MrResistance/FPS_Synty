using UnityEngine;

public class HitscanWeapon : Weapon
{
    public override void PlayWeaponFX()
    {
        base.PlayWeaponFX();
        if (m_currentlyFiring)
        {
            HitCalculation();
        }
    }

    private Vector3 HitCalculation()
    {
        // Get the center point of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Create a ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out m_raycastHit, m_effectiveRange))
        {
            if (m_raycastHit.collider.TryGetComponent<Rigidbody>(out _))
            {
                m_raycastHit.rigidbody.AddExplosionForce(m_hitForce, m_raycastHit.point, 1);
            }
            if (m_raycastHit.collider.TryGetComponent(out Damageable damageable))
            {
                damageable.LoseHitPoints(m_damage);
            }
            return m_raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
