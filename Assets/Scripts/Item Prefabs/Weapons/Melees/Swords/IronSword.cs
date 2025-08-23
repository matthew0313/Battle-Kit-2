using Unity.Netcode;
using UnityEngine;

public class IronSword : Sword
{
    [Header("IronSword")]
    [SerializeField] Transform m_flyingSlashFirePoint;
    [SerializeField] TriggerCallbacks m_circularSlashHitbox;
    public Transform flyingSlashFirePoint => m_flyingSlashFirePoint;
    public TriggerCallbacks circularSlashHitbox => m_circularSlashHitbox;

    readonly int flyingSlashID = Animator.StringToHash("FlyingSlash");
    public void FlyingSlash() => anim.SetTrigger(flyingSlashID);

    readonly int spiralSlashID = Animator.StringToHash("SpiralSlash");
    public void SpiralSlash() => anim.SetTrigger(spiralSlashID);
}