using UnityEngine;
public class RPG : Weapon
{
    [SerializeField] private GameObject m_visualRocket;
    [SerializeField] private float m_RocketForwardForce = 50f;
    public void LaunchRocket()
    {
        m_visualRocket.SetActive(false);
        var rocket = ObjectPooler.Instance.SpawnFromPool("RPG_Rocket", m_visualRocket.transform.position, m_visualRocket.transform.rotation);
        if (rocket.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(rocket.transform.forward * m_RocketForwardForce, ForceMode.Impulse);
        }
    }

    public override void ReloadComplete()
    {
        base.ReloadComplete();
        m_visualRocket.SetActive(true);
    }

    public void Empty()
    {
        //This is an empty function for the sole purpose of extending the fire animation for the RPG
    }
}
