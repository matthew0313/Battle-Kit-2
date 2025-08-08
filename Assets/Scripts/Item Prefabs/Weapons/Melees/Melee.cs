using Unity.Netcode;
using UnityEngine;

public abstract class Melee : Weapon
{
    [SerializeField] TriggerCallbacks m_hitbox;
    public TriggerCallbacks hitbox => m_hitbox;

    readonly int slashID = Animator.StringToHash("Slash");
    public void Slash()
    {
        anim.SetTrigger(slashID);
    }
}