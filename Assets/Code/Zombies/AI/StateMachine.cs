using System;

[Serializable]
public class StateMachine
{
    public IState CurrentState { get; private set; }
    public IdleState idleState;
    public PatrolState patrolState;
    public AlertState alertState;
    public ChaseState chaseState;
    public AttackState attackState;
    
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
    public StateMachine(Zombie zombie)
    {
        this.idleState = new IdleState(zombie);
        this.patrolState = new PatrolState(zombie);
        this.alertState = new AlertState(zombie);
        this.chaseState = new ChaseState(zombie);
        this.attackState = new AttackState(zombie);
    }
}
