using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public abstract class Projectile : NetworkBehaviour, ITimeSlowable
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
        counter += Time.deltaTime * speed;
        if (counter > range) NetworkObject.Despawn();
    }
    float timeScale = 1.0f;
    public void SlowTime(float scale) => SlowTimeRpc(scale);
    [Rpc(SendTo.Owner)]
    public void SlowTimeRpc(float scale) => timeScale *= scale;
    public void RevertTime(float scale) => RevertTimeRpc(scale);
    [Rpc(SendTo.Owner)]
    public void RevertTimeRpc(float scale) => timeScale /= scale;
    public bool IsEnemy(Player other) => other.IsEnemy(wielder);
}