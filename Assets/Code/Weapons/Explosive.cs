using System;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float m_explosionRadius = 2f;
    [SerializeField] private float m_explosionForce = 100f;
    public event Action OnExplode;
    private void OnCollisionEnter(Collision collision)
    {
        ObjectPooler.Instance.SpawnFromPool("SmallExplosion", transform.position, transform.rotation);
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
        }
        ExplosionPhysics();
        OnExplode?.Invoke();
        gameObject.SetActive(false);
    }

    private void ExplosionPhysics()
    {
        var objectsToMove = Physics.OverlapSphere(transform.position, m_explosionRadius);
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i].TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 1.5f);
            }
        }
    }
}
