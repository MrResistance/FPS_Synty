using UnityEngine;

public class Sniper : Weapon

{
    [SerializeField] private Camera m_sniperScopeCamera;
    public Camera SniperScopeCamera => m_sniperScopeCamera;
}
