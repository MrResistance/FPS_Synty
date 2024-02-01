using UnityEngine;

public class ChaseState : IState
{
    private Zombie m_zombie;
    public ChaseState(Zombie zombie)
    {
        this.m_zombie = zombie;
    }
    public void Enter()
    {
        // code that runs when we first enter the state
        m_zombie.CurrentState = Zombie.State.chase;
        m_zombie.Animator.ResetTrigger("Attack");
        m_zombie.NavMeshAgent.isStopped = false;
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        m_zombie.NavMeshAgent.SetDestination(m_zombie.Target);
        RunTowardsCurrentTarget();
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }

    private void RunTowardsCurrentTarget()
    {
        m_zombie.Animator.SetFloat("MoveSpeed", m_zombie.RunSpeed, 0.2f, Time.deltaTime);

        if (m_zombie.NavMeshAgent.remainingDistance <= 1.5f)
        {
            m_zombie.ZombieStateMachine.TransitionTo(m_zombie.ZombieStateMachine.attackState);
        }
    }
}
