using System;
using NUnit.Framework.Constraints;
using Unity.Netcode;
using UnityEngine;
public abstract class Bullet : Projectile, IReflectable
{
    Entity IRelationInteraction.owner => owner;

    public readonly NetworkVariable<float> damage = new(0), speed = new(0), range = new(0);
    float counter = 0.0f;

    public void Set(float damage, float speed, float range)
    {
        this.damage.Value = damage;
        this.speed.Value = speed;
        this.range.Value = range;
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
            NetworkObject.ChangeOwnership(reflector.OwnerClientId);
            RedirectRpc(dir);
        }
    }
    [Rpc(SendTo.Owner)]
    public void RedirectRpc(Vector2 dir) => transform.right = dir;
    protected virtual void Update()
    {
        if (!IsOwner) return;

        transform.Translate(transform.right * speed.Value * Time.deltaTime, Space.World);
        counter += speed.Value * Time.deltaTime;

        if (counter > range.Value) NetworkObject.Despawn();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        GameObject hit = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;
        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (owner.GetRelation(damagable.owner) == Relation.Enemy)
            {
                damagable.GetDamage(new()
                {
                    amount = damage.Value,
                    source = DamageSource.Player,
                    clientID = OwnerClientId
                });
                NetworkObject.Despawn(); return;
            }
        }

        if (hit.gameObject.layer == LayerMask.NameToLayer("Map")) NetworkObject.Despawn();
    }
}