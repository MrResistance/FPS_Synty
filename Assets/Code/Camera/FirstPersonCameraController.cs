using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    [SerializeField] private float lookSensitivity = 100f;
    [SerializeField] private float maxLookAngle = 90f;
    [SerializeField] private float lookSmoothTime = 0.1f; // Time taken to smooth the rotation
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float xRotationVelocity;
    private float yRotationVelocity;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Initialize the camera to look forward at the start.
        if (m_camera != null)
        {
            xRotation = m_camera.localEulerAngles.x;

            // Adjust for Unity's 360-degree representation.
            if (xRotation > 180)
                xRotation -= 360;
        }
    }

    private void LateUpdate()
    {
        Look(PlayerInputs.Instance.lookInput);
    }

    private void Look(Vector2 vector)
    {
        // Calculate target rotation
        float X = vector.x * lookSensitivity * Time.deltaTime;
        float Y = vector.y * lookSensitivity * Time.deltaTime;

        yRotation += X;
        xRotation -= Y;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Smoothly interpolate towards the target rotation
        float xRotationSmoothed = Mathf.SmoothDampAngle(m_camera.localEulerAngles.x, xRotation, ref xRotationVelocity, lookSmoothTime);
        float yRotationSmoothed = Mathf.SmoothDampAngle(transform.localEulerAngles.y, yRotation, ref yRotationVelocity, lookSmoothTime);

        // Apply the smoothed rotation
        m_camera.localRotation = Quaternion.Euler(xRotationSmoothed, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, yRotationSmoothed, 0f);
    }
}
