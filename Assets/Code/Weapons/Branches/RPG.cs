using UnityEngine;
public class RPG : Weapon
{
    [SerializeField] private GameObject m_visualRocket;
    [SerializeField] private float m_RocketForwardForce = 50f;
    public void SpawnRocket()
    {
        m_visualRocket.SetActive(false);
        ObjectPooler.Instance.SpawnFromPool("RPG_Rocket", m_visualRocket.transform.position, m_visualRocket.transform.rotation);
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
