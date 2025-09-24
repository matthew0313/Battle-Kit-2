using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode.Components;

public class JumpPad : MonoBehaviour
{
    [SerializeField] NetworkAnimator anim;
    [SerializeField] Vector2 knockback;
    readonly int activateID = Animator.StringToHash("Activate");
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hit = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;
        if(hit.TryGetComponent(out Player player) && player.IsOwner)
        {
            anim.SetTrigger(activateID);
            player.movements.GetKnockback(new()
            {
                value = knockback,
                source = KnockbackSource.Other
            });
        }
    }
}