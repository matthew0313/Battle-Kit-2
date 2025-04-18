using System;
using System.Collections.Generic;
using UnityEngine;

public class IronSword_FlyingSlash : Projectile
{
    float damage, acceleration;
    public void Set(Player wielder, float damage, float speed, float acceleration, float range)
    {
        base.Set(wielder, speed, range);
        this.damage = damage;
        this.acceleration = acceleration;
    }
    protected override void Update()
    {
        if (!IsOwner) return;

        speed += Time.deltaTime * acceleration;

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
        counter += Time.deltaTime * speed;

        if (counter > range) NetworkObject.Despawn();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        GameObject hit = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;

        if(hit.TryGetComponent(out Bullet bullet))
        {
            bullet.Reflect(wielder, transform.right);
            return;
        }

        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (damagable.IsEnemy(wielder))
            {
                damagable.GetDamage(new()
                {
                    amount = damage,
                    damageSource = DamageSource.Player,
                    damageGiverClientID = wielder.OwnerClientId
                });
                NetworkObject.Despawn(); return;
            }
        }

        if (hit.gameObject.layer == LayerMask.NameToLayer("Map")) NetworkObject.Despawn();
    }
}