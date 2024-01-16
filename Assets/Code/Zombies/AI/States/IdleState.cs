using UnityEngine;

public class IdleState : IState
{
    private Zombie m_zombie;
    public IdleState(Zombie zombie)
    {
        this.m_zombie = zombie;
    }
    public void Enter()
    {
        // code that runs when we first enter the state
        m_zombie.m_currentState = Zombie.State.idle;
        m_zombie.m_animator.SetFloat("MoveSpeed", 0);
        m_zombie.m_navMeshAgent.speed = 0;
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        m_zombie.zombieStateMachine.TransitionTo(m_zombie.zombieStateMachine.patrolState);
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
