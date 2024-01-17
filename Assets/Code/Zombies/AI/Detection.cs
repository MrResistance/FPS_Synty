using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Detection : MonoBehaviour
{
    [SerializeField] private Zombie m_zombie;

    private void OnTriggerEnter(Collider other)
    {
        int layer = 1 << other.gameObject.layer;
        if ((m_zombie.DetectionLayer.value & layer) != 0)
        {
            m_zombie.TargetTransform = other.transform;
            m_zombie.Target = other.transform.position;
            m_zombie.ZombieStateMachine.TransitionTo(m_zombie.ZombieStateMachine.chaseState);
        }
    }


    private void OnTriggerExit(Collider other)
    {

    }
}
