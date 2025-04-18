using Unity.Netcode;
using UnityEngine;

public abstract class Melee : Weapon
{
    [SerializeField] TriggerCallbacks m_hitbox;
    public TriggerCallbacks hitbox => m_hitbox;
    [Rpc(SendTo.Everyone)]
    public void SlashRpc()
    {
        OnSlash();
    }
    readonly int slashID = Animator.StringToHash("Slash");
    protected virtual void OnSlash()
    {
        anim.SetTrigger(slashID);
    }
}