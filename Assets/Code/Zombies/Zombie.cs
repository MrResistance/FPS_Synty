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
    [Tooltip("Smaller value is quicker.")]public float AttackSpeed = 1f;
    public int Damage;

    [Header("Targeting")]
    public LayerMask DetectionLayer;
    [Tooltip("This will usually be the player or a patrol point.")] public Vector3 Target;
    public Transform TargetTransform;
    public float DetectionRadius = 15;
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
        if (TargetTransform != null)
        {
            Target = TargetTransform.position;
        }
    }
    private void GetZombieData()
    {
        if (ZombieData != null)
        {
            RunSpeed = ZombieData.RunSpeed;
            WalkSpeed = ZombieData.WalkSpeed;
            Damage = ZombieData.Damage;
            RotationSpeed = ZombieData.RotationSpeed;
            PatrolRadius = ZombieData.PatrolRadius;
        }
        else
        {
            Debug.LogWarning("Zombie data not assigned, zombie may behave unexpectedly.");
        }
    }

    public void DealDamage()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PatrolRadius);
    }
}
