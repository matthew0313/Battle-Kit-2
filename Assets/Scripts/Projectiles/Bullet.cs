using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;
public abstract class Bullet : Projectile, IReflectable
{
    public Projectile self => this;
    float damage, speed, range;
    float counter = 0.0f;
    public void Set(float damage, float speed, float range)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
    }
    public void Reflect(Player reflector, Vector2 dir)
    {
        if (IsOwner) return;
        ReflectRpc(reflector, dir);
    }
    [Rpc(SendTo.Owner)]
    public void ReflectRpc(NetworkBehaviourReference reflectorRef, Vector2 dir)
    {
        if(reflectorRef.TryGet(out Player reflector))
        {
            counter = 0.0f;
            transform.right = dir;
            NetworkObject.ChangeOwnership(reflector.OwnerClientId);
        }
    }
    protected virtual void Update()
    {
        if (!IsOwner) return;

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
        counter += speed * Time.deltaTime;

        if (counter > range) NetworkObject.Despawn();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        GameObject hit = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;
        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (owner.GetRelation(damagable.self) == Relation.Enemy)
            {
                damagable.GetDamage(new()
                {
                    amount = damage,
                    source = DamageSource.Player,
                    clientID = OwnerClientId
                });
                NetworkObject.Despawn(); return;
            }
        }

        if (hit.gameObject.layer == LayerMask.NameToLayer("Map")) NetworkObject.Despawn();
    }
}