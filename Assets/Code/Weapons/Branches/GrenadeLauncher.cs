using System.Collections;
using UnityEngine;

public class GrenadeLauncher : Weapon
{
    [Header("Grenade Launcher Settings")]
    [SerializeField] GameObject m_grenadeAmmoPrefab;
    [SerializeField] Transform m_grenadeLauncherCylinder;
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
}
