using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Zombie : MonoBehaviour
{
    public enum State { idle, patrol, alert, chase, attack }
    public State CurrentState;
    public float RunSpeed;
    public float WalkSpeed;
    public float RotationSpeed = 5f;
    [Tooltip("Smaller value is quicker.")] public float AttackSpeed = 1f;
    public int MaxHitPoints;
    public int Damage;
    public List<Damageable> objectsToDamage;

    [Header("Targeting")]
    [Tooltip("This will usually be the player or a patrol point.")] public Vector3 Target;
    public Transform TargetTransform;
    public LayerMask DetectionLayer;
    public float DetectionRadius = 15;
    public float PatrolRadius = 10;
    public List<Vector3> PatrolPoints = new List<Vector3>();

    [Header("Audio")]
    public AudioSource AudioSource;
    public List<AudioClip> GroanSFX;
    public List<AudioClip> AttackSFX;
    public List<AudioClip> HurtSFX;
    public List<AudioClip> DeathSFX;

    [Header("References")]
    public ZombieData ZombieData;
    public Animator Animator;
    public NavMeshAgent NavMeshAgent;
    public Rigidbody Rigidbody;
    public Damageable Damageable;
    [HideInInspector] public StateMachine ZombieStateMachine;
    [SerializeField] private GameObject m_detection;
    [SerializeField] private GameObject m_attackHitbox;

    private void Start()
    {
        GetZombieData();
        ZombieStateMachine = new StateMachine(this);
        ZombieStateMachine.Initialize(ZombieStateMachine.idleState);
        Damageable.SetMaxHitPoints(MaxHitPoints);
        Damageable.GainHitPoints(MaxHitPoints);
        Damageable.OnDeath += Die;
        Damageable.OnHit += RequestHit;
    }

    private void OnEnable()
    {
        Damageable.OnDeath += Die;
        Damageable.OnHit += RequestHit;
    }
    private void OnDisable()
    {
        Damageable.OnDeath -= Die;
        Damageable.OnHit -= RequestHit;
    }
    private void OnDestroy()
    {
        Damageable.OnDeath -= Die;
        Damageable.OnHit -= RequestHit;
    }

    private void RequestHit()
    {
        StopNavMeshAgent();
        if (Damageable.HitPosition.x.CompareTo(0) > Damageable.HitPosition.z.CompareTo(0))
        {
            if (Damageable.HitPosition.x > 0)
            {
                Animator.Play("GetHitLightRightZombie");
            }
            else
            {
                Animator.Play("GetHitLightLeftZombie");
            }
        }
        else
        {
            if (Damageable.HitPosition.z > 0)
            {
                Animator.Play("GetHitLightFrontZombie");
            }
            else
            {
                Animator.Play("GetHitLightBackZombie");
            }
        }
    }

    public void Die()
    {
        Rigidbody.AddForceAtPosition(Damageable.HitPosition * 10f, Damageable.HitPosition, ForceMode.Impulse);
        Animator.enabled = false;
        NavMeshAgent.enabled = false;
        m_detection.SetActive(false);
        m_attackHitbox.SetActive(false);
    }

    private void Update()
    {
        if (Damageable.CurrentHitPoints > 0)
        {
            ZombieStateMachine.Update();
        }
        if (TargetTransform != null)
        {
            Target = TargetTransform.position;
        }
    }
    private void GetZombieData()
    {
        if (ZombieData != null)
        {
            RunSpeed = ZombieData.RunSpeed;
            WalkSpeed = ZombieData.WalkSpeed;
            Damage = ZombieData.Damage;
            RotationSpeed = ZombieData.RotationSpeed;
            PatrolRadius = ZombieData.PatrolRadius;
            MaxHitPoints = ZombieData.MaxHitPoints;
        }
        else
        {
            Debug.LogWarning("Zombie data not assigned, zombie may behave unexpectedly.");
        }
    }

    public void DealDamage()
    {
        if (objectsToDamage.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < objectsToDamage.Count; i++)
        {
            if (objectsToDamage[i].TryGetComponent(out Damageable damageable))
            {
                damageable.LoseHitPoints(Damage);
            }
        }
    }

    public void ResetTrigger(string triggerName)
    {
        Animator.ResetTrigger(triggerName);
    }

    #region Audio

    public void PlayGroanSFX()
    {
        if (GroanSFX.Count > 0)
        {
            AudioSource.PlayOneShot(GroanSFX[Random.Range(0, GroanSFX.Count)]);
        }
    }

    public void PlayAttackSFX()
    {
        if (AttackSFX.Count > 0)
        {
            AudioSource.PlayOneShot(AttackSFX[Random.Range(0, AttackSFX.Count)]);
        }
    }

    public void PlayHurtSFX()
    {
        if (HurtSFX.Count > 0)
        {
            AudioSource.PlayOneShot(HurtSFX[Random.Range(0, HurtSFX.Count)]);
        }
    }

    public void PlayDeathSFX()
    {
        if (DeathSFX.Count > 0)
        {
            AudioSource.PlayOneShot(DeathSFX[Random.Range(0, DeathSFX.Count)]);
        }
    }

    #endregion

    public void StartNavMeshAgent()
    {
        if (NavMeshAgent.isActiveAndEnabled)
        {
            NavMeshAgent.isStopped = false;
        }
    }

    public void StopNavMeshAgent()
    {
        if (NavMeshAgent.isActiveAndEnabled)
        {
            NavMeshAgent.isStopped = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PatrolRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, Target);
    }
}
