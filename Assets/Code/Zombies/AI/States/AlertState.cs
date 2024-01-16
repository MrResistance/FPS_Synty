public class AlertState : IState
{
    private Zombie m_zombie;
    public AlertState(Zombie zombie)
    {
        this.m_zombie = zombie;
    }
    public void Enter()
    {
        // code that runs when we first enter the state
        m_zombie.CurrentState = Zombie.State.alert;
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }
}
