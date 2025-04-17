using Unity.Netcode;
using UnityEngine;

public abstract class Bow : Weapon
{
    [SerializeField] Transform m_firePoint;
    [SerializeField] protected Animator anim;
    public Transform firePoint => m_firePoint;

    [Rpc(SendTo.Everyone)]
    public void FireRpc()
    {
        OnFire();
    }
    readonly int fireID = Animator.StringToHash("Fire");
    protected virtual void OnFire()
    {
        anim.SetTrigger(fireID);
    }
}