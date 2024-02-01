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
        m_zombie.CurrentState = Zombie.State.attack;
        m_zombie.Animator.SetFloat("MoveSpeed", 0);
        m_zombie.NavMeshAgent.isStopped = true;
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        if (!m_zombie.NavMeshAgent.isStopped)
        {
            m_zombie.NavMeshAgent.SetDestination(m_zombie.Target);
        }

        if (m_zombie.NavMeshAgent.remainingDistance >= 1.5f)
        {
            m_zombie.ZombieStateMachine.TransitionTo(m_zombie.ZombieStateMachine.chaseState);
            return;
        }
        
        AttackTarget();
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }

    private void AttackTarget()
    {
        if (Time.time >= m_lastTimeAttacked + m_zombie.AttackSpeed)
        {
            m_lastTimeAttacked = Time.time;
            m_zombie.Animator.SetTrigger("Attack");
        }
    }
}
