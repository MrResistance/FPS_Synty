using UnityEngine;

public class Explosive : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        ObjectPooler.Instance.SpawnFromPool("SmallExplosion", transform.position, transform.rotation);
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
        }
        gameObject.SetActive(false);
    }
}
