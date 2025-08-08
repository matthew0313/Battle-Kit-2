using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public abstract class DartItemData : StackableItemData
{
    [Header("Dart")]
    [SerializeField] Sprite m_lmbIcon;
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage;
    [SerializeField] CooldownSource m_throwCooldownSource;
    [SerializeField] float m_throwCooldown, m_dartLifetime, m_dartVelocity, m_spread;
    [SerializeField] float m_throwOffset = 1.0f;
    public Sprite lmbIcon => m_lmbIcon;
    public bool auto => m_auto;
    public float damage => m_damage;
    public CooldownSource throwCooldownSource => m_throwCooldownSource;
    public float throwCooldown => m_throwCooldown;
    public float dartLifetime => m_dartLifetime;
    public float dartVelocity => m_dartVelocity;
    public float spread => m_spread;
    public float throwOffset => m_throwOffset;
    public abstract Dart dart { get; }
}
public abstract class DartItem : StackableItem, ILMBUser
{
    new public readonly DartItemData data;
    protected DartItem(DartItemData data) : base(data)
    {
        this.data = data;
    }
    protected float counter = 0.0f;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        counter = 0.0f;
    }
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        counter += Time.deltaTime;
    }
    protected float throwCounter => data.throwCooldownSource.cooldownLeft;
    protected float throwCooldown => data.throwCooldownSource.cooldown;
    protected virtual bool canThrow => true;
    public Sprite lmbSprite => data.lmbIcon;
    public bool canUseLmb => canThrow && throwCounter <= 0.0f;
    public float lmbCooldownBarFill => throwCounter / throwCooldown;
    public void LMBUse(Player wielder, bool down)
    {
        if (!data.auto && !down || !canUseLmb) return;
        counter = 0.0f;
        Throw(wielder);
        containedSlot.count--;
    }
    protected virtual void Throw(Player wielder)
    {
        Vector2 pos = (Vector2)wielder.inventory.weaponAnchor.position + (Vector2)wielder.inventory.weaponAnchor.right * data.throwOffset;
        Quaternion rot = wielder.inventory.weaponAnchor.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.spread, data.spread));
        Dart dart = MonoBehaviour.Instantiate(data.dart, pos, rot);
        dart.Set(data.damage, data.dartLifetime, data.dartVelocity);
        dart.NetworkObject.Spawn();
    }
}