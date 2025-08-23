using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class MeleeItemData : WeaponItemData
{
    [Header("Melee")]
    [SerializeField] Sprite m_lmbIcon;
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage;
    [SerializeField] CooldownSource m_slashCooldownSource;
    [SerializeField] float m_slashCooldown;
    public Sprite lmbIcon => m_lmbIcon;
    public bool auto => m_auto;
    public float damage => m_damage;
    public CooldownSource slashCooldownSource => m_slashCooldownSource;
    public float slashCooldown => m_slashCooldown;
}
public abstract class MeleeItem : WeaponItem, ILMBUser
{
    new public readonly MeleeItemData data;
    new protected Melee weapon => base.weapon as Melee;
    protected MeleeItem(MeleeItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        weapon.hitbox.onTriggerEnter2D += OnTriggerEnter2D;
    }
    protected float slashCounter => data.slashCooldownSource.cooldownLeft;
    protected float slashCooldown => data.slashCooldownSource.cooldown;
    protected virtual bool canSlash => true;
    public Sprite lmbSprite => data.lmbIcon;
    public bool canUseLmb => canSlash && slashCounter <= 0.0f;
    public float lmbCooldownBarFill => slashCounter / slashCooldown;
    public void LMBUse(Player wielder, bool down)
    {
        if (!data.auto && !down || !canUseLmb) return;
        wielder.cooldowns.AddCooldown(data.slashCooldownSource, data.slashCooldown);
        Slash(wielder);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        weapon.hitbox.onTriggerEnter2D -= OnTriggerEnter2D;
    }
    protected virtual void Slash(Player wielder)
    {
        hitList.Clear();
        weapon.Slash();
    }
    protected List<GameObject> hitList = new();
    void OnTriggerEnter2D(Collider2D other)
    {
        if (wielder == null) return;

        GameObject hit = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (hitList.Contains(hit)) return;
        hitList.Add(hit);

        OnHit(hit);
    }
    protected virtual void OnHit(GameObject hit)
    {
        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (wielder.GetRelation(damagable.owner) == Relation.Enemy)
            {
                damagable.GetDamage(new()
                {
                    amount = data.damage * wielder.stats.attackMultiplier,
                    source = DamageSource.Player,
                    clientID = wielder.OwnerClientId
                });
            }
        }
    }
}