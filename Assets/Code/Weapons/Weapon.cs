using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private ParticleSystem m_muzzleFlashFX;
    [SerializeField] private ParticleSystem m_bulletTrailFX;
    [SerializeField] private Transform m_muzzleFlashFX_Position;
    [SerializeField] private Transform m_bulletTrailFX_Position;

    public Animator Animator => m_animator;
    public ParticleSystem MuzzleFlashFX => m_muzzleFlashFX;
    public ParticleSystem BulletTrailFX => m_bulletTrailFX;
    public Transform MuzzleFlashFX_Position => m_muzzleFlashFX_Position;
    public Transform BulletTrailFX_Position => m_bulletTrailFX_Position;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
