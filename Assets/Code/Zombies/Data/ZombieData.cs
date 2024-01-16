using UnityEngine;

[CreateAssetMenu(fileName = "New Zombie", menuName = "Zombie Menu/New Zombie", order = 2)]
public class ZombieData : ScriptableObject
{
    public float RunSpeed = 2f;
    public float WalkSpeed = 0.4f;
    public int Damage = 2;

    [Header("Targeting")]
    public float PatrolRadius = 10f;
}
