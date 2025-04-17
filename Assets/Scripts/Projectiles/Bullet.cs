using System;
using Unity.Netcode;
using UnityEngine;
public class Bullet : Projectile
{
    Action<Bullet, Collider2D> onHit;
    float speed, range;
    public void Set(Action<Bullet, Collider2D> onHit, float speed, float range)
    {
        Debug.Log("AAAAAAAAA");
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
        onHit?.Invoke(this, collision);
    }
}