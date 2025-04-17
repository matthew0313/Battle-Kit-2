using System;
using Unity.Netcode;
using UnityEngine;
public class Bullet : Projectile
{
    Player player;
    Action<Player, Bullet, GameObject> onHit;
    float speed, range;
    public void Set(Player player, Action<Player, Bullet, GameObject> onHit, float speed, float range)
    {
        this.player = player;   
        this.onHit = onHit;
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
        onHit?.Invoke(player, this, collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject);
    }
}