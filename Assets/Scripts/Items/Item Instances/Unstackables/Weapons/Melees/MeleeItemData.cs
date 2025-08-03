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
    [SerializeField] float m_slashRate;
    public Sprite lmbIcon => m_lmbIcon;
    public bool auto => m_auto;
    public float damage => m_damage;
    public float slashRate => m_slashRate;
}
public abstract class MeleeItem : WeaponItem, ILMBUser
{
    new public readonly MeleeItemData data;
    new protected Melee weapon => base.weapon as Melee;
    protected MeleeItem(MeleeItemData data) : base(data)
    {
        this.data = data;
    }
    protected float counter = 0.0f;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        weapon.hitbox.onTriggerEnter2D += OnTriggerEnter2D;
    }
    protected virtual bool canSlash => true;
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        counter += Time.deltaTime;
    }
    public Sprite lmbSprite => data.lmbIcon;
    public bool canUseLmb => canSlash && counter >= data.slashRate;
    public float lmbCooldownBarFill => 1.0f - counter / data.slashRate;
    public void LMBUse(Player wielder, bool down)
    {
        if (!data.auto && !down) return;
        if (canSlash && counter >= data.slashRate)
        {
            counter = 0.0f;
            Slash(wielder);
        }
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        weapon.hitbox.onTriggerEnter2D -= OnTriggerEnter2D;
    }
    protected virtual void Slash(Player wielder)
    {
        hitList.Clear();
        weapon.SlashRpc();
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
            if (wielder.GetRelation(damagable.self) == Relation.Enemy)
            {
                damagable.GetDamage(new()
                {
                    amount = data.damage,
                    source = DamageSource.Player,
                    clientID = wielder.OwnerClientId
                });
            }
        }
    }
}