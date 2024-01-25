using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int m_maxHitPoints;
    [SerializeField] private int m_currentHitPoints;

    public event Action<int> OnHitPointsChange;
    public event Action OnDeath;
    private void Start()
    {
        m_currentHitPoints = m_maxHitPoints;
    }

    public void SetMaxHitPoints(int amount)
    {
        m_maxHitPoints = amount;
    }

    public void GainHitPoints(int amount)
    {
        m_currentHitPoints += amount;
        if (m_currentHitPoints >= m_maxHitPoints)
        {
            m_currentHitPoints = m_maxHitPoints;
        }
        OnHitPointsChange?.Invoke(m_currentHitPoints);
    }

    public void LoseHitPoints(int amount)
    {
        m_currentHitPoints -= amount;
        if (m_currentHitPoints <= 0)
        {
            m_currentHitPoints = 0;
            Die();
        }
        OnHitPointsChange?.Invoke(m_currentHitPoints);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        //gameObject.SetActive(false);
    }
}
