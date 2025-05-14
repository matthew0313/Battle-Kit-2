using Unity.Netcode;
using UnityEngine;

public class IronSword : Sword
{
    [Header("IronSword")]
    [SerializeField] Transform m_flyingSlashFirePoint;
    [SerializeField] TriggerCallbacks m_circularSlashHitbox;
    public Transform flyingSlashFirePoint => m_flyingSlashFirePoint;
    public TriggerCallbacks circularSlashHitbox => m_circularSlashHitbox;
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
    [Rpc(SendTo.Everyone)]
    public void SpiralSlashRpc()
    {
        OnSpiralSlash();
    }
    readonly int spiralSlashID = Animator.StringToHash("SpiralSlash");
    void OnSpiralSlash()
    {
        anim.SetTrigger(spiralSlashID);
    }
}