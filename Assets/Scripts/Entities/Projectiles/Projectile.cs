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

        if (timeScale < targetTimeScale) timeScale = Mathf.Min(timeScale + timeScaleRate * Time.deltaTime, targetTimeScale);
        if (timeScale > targetTimeScale) timeScale = Mathf.Max(timeScale - timeScaleRate * Time.deltaTime, targetTimeScale);

        transform.Translate(transform.right * speed * Time.deltaTime * timeScale, Space.World);
        counter += speed * Time.deltaTime * timeScale;

        if (counter > range) NetworkObject.Despawn();
    }
    const float timeScaleRate = 3.0f;
    float targetTimeScale = 1.0f;
    float timeScale = 1.0f;
    public void SlowTime(float scale) => SlowTimeRpc(scale);
    [Rpc(SendTo.Authority)]
    public void SlowTimeRpc(float scale) => targetTimeScale *= scale;
    public void RevertTime(float scale) => RevertTimeRpc(scale);
    [Rpc(SendTo.Authority)]
    public void RevertTimeRpc(float scale) => targetTimeScale /= scale;
    public bool IsEnemy(Player other) => other.IsEnemy(wielder);
}