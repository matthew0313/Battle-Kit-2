using System;
using System.Collections.Generic;
using UnityEngine;

public class IronSword_FlyingSlash : Projectile
{
    float damage, acceleration, knockback, slowPercentage, slowDuration;
    public void Set(Player wielder, float damage, float speed, float acceleration, float range, float knockback, float slowPercentage, float slowDuration)
    {
        base.Set(wielder, speed, range);
        this.damage = damage;
        this.acceleration = acceleration;
        this.knockback = knockback;
        this.slowPercentage = slowPercentage;
        this.slowDuration = slowDuration;
    }
    protected override void Update()
    {
        if (!IsOwner) return;

        speed += Time.deltaTime * acceleration;

        transform.Translate(transform.right * (transform.localScale.x > 0.0f ? 1.0f : -1.0f) * speed * Time.deltaTime, Space.World);
        counter += Time.deltaTime * speed;

        if (counter > range) NetworkObject.Despawn();
    }
    List<GameObject> hit = new();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        GameObject hit = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;

        if (this.hit.Contains(hit)) return;
        this.hit.Add(hit);

        if(hit.TryGetComponent(out Bullet bullet))
        {
            bullet.Reflect(wielder, transform.right);
            return;
        }

        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (damagable.IsEnemy(wielder))
            {
                if(damagable is HpComp hpComp) hpComp.AddAliment(new Slowed(slowDuration, slowPercentage));
                damagable.GetDamage(new()
                {
                    amount = damage,
                    source = DamageSource.Player,
                    clientID = wielder.OwnerClientId
                });
            }
        }

        if(hit.TryGetComponent(out IKnockable knockable))
        {
            if (knockable.IsEnemy(wielder))
            {
                knockable.GetKnockback(new()
                {
                    value = transform.right * knockback,
                    source = KnockbackSource.Player,
                    clientID = wielder.OwnerClientId
                });
            }
        }
    }
}