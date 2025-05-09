using System;
using Unity.Netcode;
using UnityEngine;
public abstract class Bullet : Projectile
{
    float damage;
    public void Set(Player wielder, float damage, float speed, float range)
    {
        base.Set(wielder, speed, range);
        this.damage = damage;
    }
    public void Reflect(Player reflector, Vector2 dir)
    {
        counter = 0.0f;
        transform.right = dir;
        NetworkObject.ChangeOwnership(reflector.OwnerClientId);
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        GameObject hit = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;
        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (damagable.IsEnemy(wielder))
            {
                damagable.GetDamage(new()
                {
                    amount = damage,
                    source = DamageSource.Player,
                    clientID = wielder.OwnerClientId
                });
                NetworkObject.Despawn(); return;
            }
        }

        if (hit.gameObject.layer == LayerMask.NameToLayer("Map")) NetworkObject.Despawn();
    }
}