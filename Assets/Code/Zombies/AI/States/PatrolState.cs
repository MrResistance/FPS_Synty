using UnityEngine;

public class PatrolState : IState
{
    private Zombie m_zombie;
    public PatrolState(Zombie zombie)
    {
        this.m_zombie = zombie;
    }
    public void Enter()
    {
        // code that runs when we first enter the state
        m_zombie.m_currentState = Zombie.State.patrol;
        GetPatrolPoints();
    }
    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        GoToPatrolPoint();
    }
    public void Exit()
    {
        // code that runs when we exit the state
    }

    private void GetPatrolPoints()
    {
        var potentialPatrolPoints = Physics.OverlapSphere(m_zombie.transform.position, m_zombie.m_patrolRadius);
        for (int i = 0; i < potentialPatrolPoints.Length; i++)
        {
            if (potentialPatrolPoints[i].TryGetComponent(out PatrolPoint patrolPoint))
            {
                m_zombie.m_patrolPoints.Add(potentialPatrolPoints[i].transform.position);
            }
        }
        if (potentialPatrolPoints.Length > 0)
        {
            int rand = Random.Range(0, potentialPatrolPoints.Length);
            m_zombie.Target = potentialPatrolPoints[rand].transform.position;
            m_zombie.m_navMeshAgent.SetDestination(m_zombie.Target);
        }
        else
        {
            m_zombie.Target = GetRandomPosition();
            m_zombie.m_navMeshAgent.SetDestination(m_zombie.Target);
        }
    }

    private void GoToPatrolPoint()
    {
        Debug.Log("Remaining distance: " + m_zombie.m_navMeshAgent.remainingDistance);
        if (m_zombie.m_navMeshAgent.remainingDistance <= 0.1)
        {
            m_zombie.Target = GetNextPatrolPoint();
            m_zombie.m_navMeshAgent.SetDestination(m_zombie.Target);
        }
        else
        {
            m_zombie.m_navMeshAgent.speed = m_zombie.m_moveSpeed;
            m_zombie.m_animator.SetFloat("MoveSpeed", m_zombie.m_moveSpeed);
        }
    }

    private Vector3 GetNextPatrolPoint()
    {
        Debug.Log("Getting next patrol point...");
        if (m_zombie.m_patrolPoints.Count > 0)
        {
            for (int i = 0; i < m_zombie.m_patrolPoints.Count; i++)
            {
                if (m_zombie.m_patrolPoints[i] == m_zombie.Target)
                {
                    if (i + 1 >= m_zombie.m_patrolPoints.Count)
                    {
                        return m_zombie.m_patrolPoints[0];
                    }
                    else
                    {
                        return m_zombie.m_patrolPoints[i + 1];
                    }
                }
            }
            int rand = Random.Range(0, m_zombie.m_patrolPoints.Count);
            return m_zombie.m_patrolPoints[rand];
        }
        Debug.Log("Could not find specific patrol point, picking random spot...");
        return GetRandomPosition();
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(m_zombie.transform.localPosition.x - 10, m_zombie.transform.localPosition.x + 10);
        float randomZ = Random.Range(m_zombie.transform.localPosition.z - 10, m_zombie.transform.localPosition.z + 10);
        return new Vector3(randomX, 0, randomZ);
    }
}
