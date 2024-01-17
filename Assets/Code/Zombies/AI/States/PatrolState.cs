using Unity.VisualScripting;
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
        InitialisePatrolSettings();
        GetPatrolPoints();
    }

    public void Update()
    {
        // Here we add logic to detect if the conditions exist to
        // transition to another state
        WalkTowardsCurrentTarget();
        RotateTowardsTarget();
    }

    public void Exit()
    {
        // code that runs when we exit the state
        ClearPatrolPoints();
    }

    private void GetPatrolPoints()
    {
        var potentialPatrolPoints = Physics.OverlapSphere(m_zombie.transform.position, m_zombie.PatrolRadius);
        for (int i = 0; i < potentialPatrolPoints.Length; i++)
        {
            if (potentialPatrolPoints[i].TryGetComponent(out PatrolPoint patrolPoint))
            {
                m_zombie.PatrolPoints.Add(potentialPatrolPoints[i].transform.position);
            }
        }
        if (potentialPatrolPoints.Length > 0)
        {
            int rand = Random.Range(0, potentialPatrolPoints.Length);
            m_zombie.Target = potentialPatrolPoints[rand].transform.position;
        }
        else
        {
            m_zombie.Target = GetRandomPosition();
        }
    }

    private void WalkTowardsCurrentTarget()
    {
        m_zombie.Animator.SetFloat("MoveSpeed", m_zombie.WalkSpeed, 0.2f, Time.deltaTime);

        if (m_zombie.NavMeshAgent.remainingDistance <= 0.1)
        {
            m_zombie.Target = GetNextPatrolPoint();
        }
    }

    private void RotateTowardsTarget()
    {
        m_zombie.NavMeshAgent.enabled = true;
        m_zombie.NavMeshAgent.SetDestination(m_zombie.Target);
        m_zombie.transform.rotation = Quaternion.Slerp(m_zombie.transform.rotation, m_zombie.NavMeshAgent.transform.rotation, m_zombie.RotationSpeed / Time.deltaTime);
    }

    private Vector3 GetNextPatrolPoint()
    {
        if (m_zombie.PatrolPoints.Count > 0)
        {
            for (int i = 0; i < m_zombie.PatrolPoints.Count; i++)
            {
                if (m_zombie.PatrolPoints[i] == m_zombie.Target)
                {
                    if (i + 1 >= m_zombie.PatrolPoints.Count)
                    {
                        return m_zombie.PatrolPoints[0];
                    }
                    else
                    {
                        return m_zombie.PatrolPoints[i + 1];
                    }
                }
            }
            int rand = Random.Range(0, m_zombie.PatrolPoints.Count);
            return m_zombie.PatrolPoints[rand];
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

    private void InitialisePatrolSettings()
    {
        m_zombie.CurrentState = Zombie.State.patrol;
    }
    private void ClearPatrolPoints()
    {
        m_zombie.PatrolPoints.Clear();
    }
}
