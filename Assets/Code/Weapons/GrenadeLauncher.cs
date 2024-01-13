using System.Collections;
using UnityEngine;

public class GrenadeLauncher : Weapon
{
    [Header("Grenade Launcher Settings")]
    [SerializeField] GameObject m_grenadeAmmoPrefab;
    [SerializeField] Transform m_grenadeLauncherCylinder;
    [SerializeField] float m_grenadeForwardForce = 20f;
    [SerializeField] float m_grenadeUpwardForce = 5f;
    [SerializeField] float m_barrelRotateSpeed = 300f;
    private Quaternion m_targetRotation;
    private float currentRotationAngle = 0f; // Tracks the cumulative rotation

    public Transform GrenadeLauncherCylinder => m_grenadeLauncherCylinder;

    public void RotateGrenadeLauncherCylinder(float rotateTarget)
    {
        // Increment the cumulative rotation
        currentRotationAngle += rotateTarget;

        // Calculate the target rotation as a Quaternion
        m_targetRotation = Quaternion.Euler(m_grenadeLauncherCylinder.transform.localRotation.x, m_grenadeLauncherCylinder.transform.localRotation.y, currentRotationAngle);

        StartCoroutine(RotateGrenadeLauncherCylinderCoroutine());
    }

    private IEnumerator RotateGrenadeLauncherCylinderCoroutine()
    {
        // Continue rotating until the target rotation is reached
        while (Quaternion.Angle(m_grenadeLauncherCylinder.transform.localRotation, m_targetRotation) > 0.01f)
        {
            // Rotate towards the target rotation
            m_grenadeLauncherCylinder.transform.localRotation = Quaternion.RotateTowards(m_grenadeLauncherCylinder.transform.localRotation, m_targetRotation, m_barrelRotateSpeed * Time.deltaTime);

            // Wait until the next frame
            yield return null;
        }

        // Optionally, snap to the exact target rotation
        m_grenadeLauncherCylinder.transform.localRotation = m_targetRotation;
    }

    public void LaunchGrenade()
    {
        var grenade = ObjectPooler.Instance.SpawnFromPool("GrenadeLauncherAmmo", Barrel.transform.position, Barrel.transform.rotation);
        if (grenade.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(transform.forward * m_grenadeForwardForce, ForceMode.Impulse);
            rb.AddForce(transform.up * m_grenadeUpwardForce, ForceMode.Impulse);
        }
    }

    public void Empty()
    {
        //This is an empty function for the sole purpose of extending the fire animation for the grenade launcher
    }
}
