using Unity.Netcode;
using UnityEngine;

public abstract class Gun : Weapon
{
    [Header("Gun")]
    [SerializeField] Transform m_firePoint;
    public Transform firePoint => m_firePoint;

    readonly int fireID = Animator.StringToHash("Fire");
    readonly int reloadID = Animator.StringToHash("Reload");
    [Rpc(SendTo.Everyone)]
    public void FireRpc() => anim.SetTrigger(fireID);
    [Rpc(SendTo.Everyone)]
    public void ReloadRpc() => anim.SetTrigger(reloadID);
}