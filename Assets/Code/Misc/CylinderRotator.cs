using System.Collections;
using UnityEngine;

public class CylinderRotator : MonoBehaviour
{
    [SerializeField] Transform m_cylinder;
    [SerializeField] float m_rotateSpeed = 300f;
    private Quaternion m_targetRotation;
    private float currentRotationAngle = 0f; // Tracks the cumulative rotation

    public void RotateCylinder(float rotateTarget)
    {
        // Increment the cumulative rotation
        currentRotationAngle += rotateTarget;

        // Calculate the target rotation as a Quaternion
        m_targetRotation = Quaternion.Euler(m_cylinder.transform.localRotation.x, m_cylinder.transform.localRotation.y, currentRotationAngle);

        StartCoroutine(RotateCylinderCoroutine());
    }

    private IEnumerator RotateCylinderCoroutine()
    {
        // Continue rotating until the target rotation is reached
        while (Quaternion.Angle(m_cylinder.transform.localRotation, m_targetRotation) > 0.01f)
        {
            // Rotate towards the target rotation
            m_cylinder.transform.localRotation = Quaternion.RotateTowards(m_cylinder.transform.localRotation, m_targetRotation, m_rotateSpeed * Time.deltaTime);

            // Wait until the next frame
            yield return null;
        }

        // Optionally, snap to the exact target rotation
        m_cylinder.transform.localRotation = m_targetRotation;
    }
}
