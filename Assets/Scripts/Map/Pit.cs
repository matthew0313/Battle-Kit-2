using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class Pit : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float knockback;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hit = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;
        if (hit.TryGetComponent(out Player player) && player.IsOwner)
        {
            player.hpComp.GetDamage(new()
            {
                amount = damage,
                source = DamageSource.Other,
                ignoreDefense = true
            });
            player.movements.GetKnockback(new()
            {
                value = Vector2.up * knockback,
                source = KnockbackSource.Other
            });
        }
    }
}