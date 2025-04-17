using Unity.Netcode;
using UnityEngine;

public abstract class Gun : Weapon
{
    [SerializeField] Transform m_firePoint;
    [SerializeField] protected Animator anim;
    public Transform firePoint => m_firePoint;

    [Rpc(SendTo.Everyone)]
    public void FireRpc()
    {
        OnFire();
    }
    [Rpc(SendTo.Everyone)]
    public void ReloadRpc()
    {
        OnReload();
    }
    readonly int fireID = Animator.StringToHash("Fire");
    protected virtual void OnFire()
    {
        anim.SetTrigger(fireID);
    }
    readonly int reloadID = Animator.StringToHash("Reload");
    protected virtual void OnReload()
    {
        anim.SetTrigger(reloadID);
    }
}