using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public abstract class Gun : Weapon
{
    [Header("Gun")]
    [SerializeField] Transform m_firePoint;
    public Transform firePoint => m_firePoint;

    readonly int fireID = Animator.StringToHash("Fire");
    readonly int reloadID = Animator.StringToHash("Reload");
    public void Fire() => anim.SetTrigger(fireID);
    public void Reload() => anim.SetTrigger(reloadID);
}