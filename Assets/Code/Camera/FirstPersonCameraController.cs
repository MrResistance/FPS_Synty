using System;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_camera;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInputs.Instance.OnLook += Look;
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
        m_camera.localRotation = new Quaternion(vector.y, vector.x, 0, m_camera.localRotation.w);
    }
}
