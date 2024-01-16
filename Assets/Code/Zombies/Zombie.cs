using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Zombie : MonoBehaviour
{
    public enum State { idle, patrol, alert, chase, attack }
    public State CurrentState;
    public float RunSpeed;
    public float WalkSpeed;
    public float RotationSpeed = 5f;
    public int Damage;

    [Header("Targeting")]
    [Tooltip("This will usually be the player or a patrol point.")] public Vector3 Target;
    public float PatrolRadius = 10;
    public List<Vector3> PatrolPoints = new List<Vector3>();

    [Header("References")]
    public ZombieData ZombieData;
    public Animator Animator;
    public NavMeshAgent NavMeshAgent;
    public Rigidbody Rigidbody;
    public StateMachine ZombieStateMachine;

    private void Start()
    {
        GetZombieData();
        ZombieStateMachine = new StateMachine(this);
        ZombieStateMachine.Initialize(ZombieStateMachine.idleState);
    }
    private void Update()
    {
        ZombieStateMachine.Update();
        NavMeshAgent.transform.localPosition = Vector3.zero;
    }
    private void GetZombieData()
    {
        if (ZombieData != null)
        {
            RunSpeed = ZombieData.RunSpeed;
            WalkSpeed = ZombieData.WalkSpeed;
            Damage = ZombieData.Damage;
            PatrolRadius = ZombieData.PatrolRadius;
        }
        else
        {
            Debug.LogWarning("Zombie data not assigned, zombie may behave unexpectedly.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PatrolRadius);
    }
}
