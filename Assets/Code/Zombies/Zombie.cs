using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public enum State { idle, patrol, alert, chase, attack }
    public State m_currentState;
    public float m_runSpeed;
    public float m_walkSpeed;
    public int m_damage;

    [Header("Targeting")]
    [Tooltip("This will usually be the player or a patrol point.")] public Vector3 Target;
    public float m_patrolRadius = 10;
    public List<Vector3> m_patrolPoints = new List<Vector3>();

    [Header("References")]
    public ZombieData m_zombieData;
    public Animator m_animator;
    public NavMeshAgent m_navMeshAgent;
    public StateMachine zombieStateMachine;

    private void Start()
    {
        GetZombieData();
        zombieStateMachine = new StateMachine(this);
        zombieStateMachine.Initialize(zombieStateMachine.idleState);
    }
    private void Update()
    {
        zombieStateMachine.Update();
    }
    private void GetZombieData()
    {
        if (m_zombieData != null)
        {
            m_runSpeed = m_zombieData.RunSpeed;
            m_walkSpeed = m_zombieData.WalkSpeed;
            m_damage = m_zombieData.Damage;
            m_patrolRadius = m_zombieData.PatrolRadius;
        }
        else
        {
            Debug.LogWarning("Zombie data not assigned, zombie may behave unexpectedly.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_patrolRadius);
    }
}
