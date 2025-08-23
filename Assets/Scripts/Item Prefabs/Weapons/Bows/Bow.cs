using Unity.Netcode;
using UnityEngine;

public abstract class Bow : Weapon
{
    [Header("Bow")]
    [SerializeField] Transform m_firePoint;
    public Transform firePoint => m_firePoint;

    readonly int fireID = Animator.StringToHash("Fire");
    public void Fire() => anim.SetTrigger(fireID);
}