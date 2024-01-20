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
        m_zombie.CurrentState = Zombie.State.idle;
        m_zombie.Animator.SetFloat("MoveSpeed", 0);
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        
        m_zombie.ZombieStateMachine.TransitionTo(m_zombie.ZombieStateMachine.patrolState);
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }
}
