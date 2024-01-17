using UnityEngine;

public class AttackState : IState
{
    private Zombie m_zombie;

    private float m_lastTimeAttacked;
    public AttackState(Zombie zombie)
    {
        this.m_zombie = zombie;
    }
    public void Enter()
    {
        // code that runs when we first enter the state
        Debug.Log("Entering Attack State");
        m_zombie.CurrentState = Zombie.State.attack;
        //m_zombie.Animator.SetTrigger("Attack");
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        m_zombie.Animator.SetFloat("MoveSpeed", 0, 0.2f, Time.deltaTime);

        if (m_zombie.NavMeshAgent.remainingDistance >= 1.5f)
        {
            m_zombie.ZombieStateMachine.TransitionTo(m_zombie.ZombieStateMachine.chaseState);
        }
        RotateTowardsTarget();
        AttackTarget();
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }

    private void AttackTarget()
    {
        Debug.Log("Requesting attack target");
        if (Time.time >= m_lastTimeAttacked + m_zombie.AttackSpeed)
        {
            Debug.Log("Attack request granted");
            m_lastTimeAttacked = Time.time;
            m_zombie.Animator.SetTrigger("Attack");
        }
    }

    private void RotateTowardsTarget()
    {
        m_zombie.NavMeshAgent.enabled = true;
        m_zombie.NavMeshAgent.SetDestination(m_zombie.Target);
        m_zombie.transform.rotation = Quaternion.Slerp(m_zombie.transform.rotation, m_zombie.NavMeshAgent.transform.rotation, m_zombie.RotationSpeed / Time.deltaTime);
    }
}
