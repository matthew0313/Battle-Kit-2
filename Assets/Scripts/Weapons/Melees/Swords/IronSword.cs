using Unity.Netcode;
using UnityEngine;

public class IronSword : Sword
{
    [Header("IronSword")]
    [SerializeField] Transform m_flyingSlashFirePoint;
    public Transform flyingSlashFirePoint => m_flyingSlashFirePoint;
    [Rpc(SendTo.Everyone)]
    public void FlyingSlashRpc()
    {
        OnFlyingSlash();
    }
    readonly int flyingSlashID = Animator.StringToHash("FlyingSlash");
    void OnFlyingSlash()
    {
        anim.SetTrigger(flyingSlashID);
    }
}