using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkRigidbody2D))]
public abstract class Shuriken : Projectile, IReflectable
{
    Entity IRelationInteraction.owner => owner;
    public Rigidbody2D rb { get; private set; }

    public readonly NetworkVariable<float> damage = new(0), lifetime = new(0);
    float counter = 0.0f;
    public void Set(float damage, float lifetime, float velocity)
    {
        this.damage.Value = damage;
        this.lifetime.Value = lifetime;
        rb.linearVelocity = transform.right * velocity;
    }
    public void Reflect(Player reflector, Vector2 dir)
    {
        if (IsOwner) return;
        ReflectRpc(reflector, dir);
    }
    [Rpc(SendTo.Owner)]
    public void ReflectRpc(NetworkBehaviourReference reflectorRef, Vector2 dir)
    {
        if (reflectorRef.TryGet(out Player reflector))
        {
            counter = 0.0f;
            Vector2 velocity = dir.normalized * rb.linearVelocity.magnitude;
            NetworkObject.ChangeOwnership(reflector.OwnerClientId);
            SetVelocityRpc(velocity);
        }
    }
    [Rpc(SendTo.Owner)]
    public void SetVelocityRpc(Vector2 velocity) => rb.linearVelocity = velocity;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual bool syncRot => true;
    protected virtual void Update()
    {
        if (!IsOwner) return;
        counter += Time.deltaTime;
        Vector2 dir = rb.linearVelocity.normalized;
        if(syncRot) transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        if (counter > lifetime.Value) NetworkObject.Despawn();
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
                    amount = damage.Value * owner.stats.attackMultiplier,
                    source = DamageSource.Player,
                    clientID = OwnerClientId
                });
                NetworkObject.Despawn(); return;
            }
        }

        if (hit.gameObject.layer == LayerMask.NameToLayer("Map")) NetworkObject.Despawn();
    }
}