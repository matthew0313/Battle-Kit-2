using System;
using System.Collections.Generic;
using UnityEngine;

public class IronSword_FlyingSlash : Projectile
{
    float damage, speed, range, acceleration, knockback, slowPercentage, slowDuration;
    float counter = 0.0f;
    public void Set(Player wielder, float damage, float speed, float acceleration, float range, float knockback, float slowPercentage, float slowDuration)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.acceleration = acceleration;
        this.knockback = knockback;
        this.slowPercentage = slowPercentage;
        this.slowDuration = slowDuration;
    }
    private void Update()
    {
        if (!IsOwner) return;

        speed += Time.deltaTime * acceleration;

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
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

        if(hit.TryGetComponent(out IReflectable reflectable))
        {
            if(owner.GetRelation(reflectable.owner) == Relation.Enemy)
            {
                reflectable.Reflect(owner, transform.right);
                return;
            }
        }

        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (owner.GetRelation(damagable.owner) == Relation.Enemy)
            {
                damagable.GetDamage(new()
                {
                    amount = damage * owner.stats.attackMultiplier,
                    source = DamageSource.Player,
                    clientID = OwnerClientId
                });
            }
        }
        
        if(hit.TryGetComponent(out IAlimentable alimentable))
        {
            if(owner.GetRelation(damagable.owner) == Relation.Enemy)
            {
                alimentable.AddAliment(new Slowed_Aliment(slowDuration, slowPercentage));
            }
        }

        if(hit.TryGetComponent(out IKnockable knockable))
        {
            if (owner.GetRelation(knockable.owner) == Relation.Enemy)
            {
                knockable.GetKnockback(new()
                {
                    value = transform.right * (transform.localScale.x > 0.0f ? 1.0f : -1.0f) * knockback,
                    source = KnockbackSource.Player,
                    clientID = OwnerClientId
                });
            }
        }
    }
}