using System;
using Unity.Netcode;
using UnityEngine;
public class Bullet : Projectile
{
    Player wielder;
    float damage, speed, range;
    public void Set(Player wielder, float damage, float speed, float range)
    {
        this.wielder = wielder;
        this.damage = damage;
        this.speed = speed;
        this.range = range;
    }
    float counter = 0.0f;
    private void Update()
    {
        if (!IsOwner) return;
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
        counter += Time.deltaTime;
        if (counter > range) NetworkObject.Despawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
                    damageSource = DamageSource.Player,
                    damageGiverClientID = wielder.OwnerClientId
                });
                NetworkObject.Despawn(); return;
            }
        }

        if (hit.gameObject.layer == LayerMask.NameToLayer("Map")) NetworkObject.Despawn();
    }
}