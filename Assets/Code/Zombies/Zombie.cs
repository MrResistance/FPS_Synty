using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] protected float m_moveSpeed;
    [SerializeField] protected int m_damage;

    public enum State { idle, patrolling, searching, pursuing, attacking }
    public State m_currentState;

    private void Update()
    {
        switch (m_currentState)
        {
            case State.idle:
                IdleBehavior();
                break;
            case State.patrolling:
                PatrollingBehavior();
                break;
            case State.searching:
                SearchingBehavior();
                break;
            case State.pursuing:
                PursuingBehavior();
                break;
            case State.attacking:
                AttackingBehavior();
                break;
        }
    }

    private void IdleBehavior()
    {
        // Idle behavior logic
        // Transition to Patrolling based on condition
    }

    private void PatrollingBehavior()
    {
        // Patrolling behavior logic
        // Transition to Searching or Idle based on condition
    }

    private void SearchingBehavior()
    {
        // Searching behavior logic
        // Transition to Pursuing based on condition
    }

    private void PursuingBehavior()
    {
        // Pursuing behavior logic
        // Transition to Attacking or Searching based on condition
    }

    private void AttackingBehavior()
    {
        // Attacking behavior logic
        // Transition to Idle or Patrolling based on condition
    }

    // Method to change states
    public void ChangeState(State newState)
    {
        m_currentState = newState;
        // Any additional logic on changing states
    }
}
