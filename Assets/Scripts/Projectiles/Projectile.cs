using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[RequireComponent(typeof(NetworkTransform))]
public abstract class Projectile : NetworkBehaviour
{
    protected Player wielder;
    protected float speed, range;
    public void Set(Player wielder, float speed, float range)
    {
        this.wielder = wielder;
        this.speed = speed;
        this.range = range;
    }
    protected float counter = 0.0f;
    protected virtual void Update()
    {
        if (!IsOwner) return;

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
        counter += Time.deltaTime;
        if (counter > range) NetworkObject.Despawn();
    }
}