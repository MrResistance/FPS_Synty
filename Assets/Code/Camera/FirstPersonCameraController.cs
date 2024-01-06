using System;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private float lookSensitivity = 100f;
    [SerializeField] private float maxLookAngle = 90f;
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInputs.Instance.OnLook += Look;
        // Initialize the camera to look forward at the start.
        if (m_camera != null)
        {
            xRotation = m_camera.localEulerAngles.x;

            // Adjust for Unity's 360-degree representation.
            if (xRotation > 180)
                xRotation -= 360;
        }
    }

    private void OnEnable()
    {
        if (PlayerInputs.Instance == null) return;
        PlayerInputs.Instance.OnLook -= Look;
        PlayerInputs.Instance.OnLook += Look;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnLook -= Look;
    }

    private void OnDestroy()
    {
        PlayerInputs.Instance.OnLook -= Look;
    }

    private void Look(Vector2 vector)
    {
        // Only update the rotation if there is input.
        if (vector != Vector2.zero)
        {
            // Calculate rotation about the Y axis (turning left/right)
            float mouseX = vector.x * lookSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);

            // Calculate rotation about the X axis (looking up/down)
            float mouseY = vector.y * lookSensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

            // Apply rotation about the X axis
            m_camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
