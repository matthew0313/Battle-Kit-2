using System;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "IronSword", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Swords/IronSword")]
public class IronSword_ItemData : SwordItemData
{
    [Header("IronSword")]
    [SerializeField] IronSword m_weaponPrefab;
    public override Weapon weaponPrefab => m_weaponPrefab;

    [Header("FlyingSlash")]
    [SerializeField] Sprite m_flyingSlashSprite;
    [SerializeField] IronSword_FlyingSlash m_flyingSlash;
    [SerializeField] float m_flyingSlashDamage, m_flyingSlashSpeed, m_flyingSlashRange, m_flyingSlashAcceleration;
    [SerializeField] CooldownSource m_flyingSlashCooldownSource;
    [SerializeField] float m_flyingSlashCooldown, m_flyingSlashKnockback, m_flyingSlashSlowPercentage, m_flyingSlashSlowDuration;
    [SerializeField] UnitTime m_flyingSlashDelay = 0.5f, m_flyingSlashDuration = 1.0f;
    public Sprite flyingSlashSprite => m_flyingSlashSprite;
    public IronSword_FlyingSlash flyingSlash => m_flyingSlash;
    public float flyingSlashDamage => m_flyingSlashDamage;
    public float flyingSlashSpeed => m_flyingSlashSpeed;
    public float flyingSlashAcceleration => m_flyingSlashAcceleration;
    public float flyingSlashRange => m_flyingSlashRange;
    public CooldownSource flyingSlashCooldownSource => m_flyingSlashCooldownSource;
    public float flyingSlashCooldown => m_flyingSlashCooldown;
    public float flyingSlashKnockback => m_flyingSlashKnockback;
    public float flyingSlashSlowPercentage => m_flyingSlashSlowPercentage;
    public float flyingSlashSlowDuration => m_flyingSlashSlowDuration;
    public float flyingSlashDelay => m_flyingSlashDelay;
    public float flyingSlashDuration => m_flyingSlashDuration;

    [Header("SpiralSlash")]
    [SerializeField] Sprite m_spiralSlashSprite;
    [SerializeField] CooldownSource m_spiralSlashCooldownSource;
    [SerializeField] float m_spiralSlashCooldown, m_spiralSlashKnockback;
    [SerializeField] UnitTime m_spiralSlashDuration;
    public Sprite spiralSlashSprite => m_spiralSlashSprite;
    public CooldownSource spiralSlashCooldownSource => m_spiralSlashCooldownSource;
    public float spiralSlashCooldown => m_spiralSlashCooldown;
    public float spiralSlashKnockback => m_spiralSlashKnockback;
    public float spiralSlashDuration => m_spiralSlashDuration;
    public override Item Create() => new IronSword_Item(this);
}
public class IronSword_Item : SwordItem, ISkill1User, ISkill2User
{
    new public readonly IronSword_ItemData data;
    new protected IronSword weapon => base.weapon as IronSword;

    public IronSword_Item(IronSword_ItemData data) : base(data)
    {
        this.data = data;
    }

    //flyingSlash
    float flyingSlashCounter => data.flyingSlashCooldownSource.cooldownLeft;
    float flyingSlashCooldown => data.flyingSlashCooldownSource.cooldown;
    bool usingFlyingSlash = false;
    public Sprite skill1Sprite => data.flyingSlashSprite;
    public bool canUseSkill1 => flyingSlashCounter <= 0.0f && slashCounter <= 0.0f && !usingSpiralSlash;
    public float skill1CooldownBarFill => flyingSlashCounter / flyingSlashCooldown;
    CoroutineHandle flyingSlashCoroutine;

    //spiralSlash
    float spiralSlashCounter => data.spiralSlashCooldownSource.cooldownLeft;
    float spiralSlashCooldown => data.spiralSlashCooldownSource.cooldown;
    bool usingSpiralSlash = false;
    public Sprite skill2Sprite => data.spiralSlashSprite;
    public bool canUseSkill2 => spiralSlashCounter <= 0.0f && slashCounter <= 0.0f && !usingFlyingSlash;
    public float skill2CooldownBarFill => spiralSlashCounter / spiralSlashCooldown;
    CoroutineHandle spiralSlashEndCoroutine;
    protected override bool canSlash => base.canSlash && !usingFlyingSlash && !usingSpiralSlash;
    public void Skill1Use(Player wielder)
    {
        if (!canUseSkill1) return;
        wielder.cooldowns.AddCooldown(data.flyingSlashCooldownSource, data.flyingSlashCooldown);
        flyingSlashCoroutine = Timing.RunCoroutine(FlyingSlash());
    }
    protected override bool canFlip => base.canFlip && !usingSpiralSlash;
    public void Skill2Use(Player wielder)
    {
        if (!canUseSkill2) return;
        wielder.cooldowns.AddCooldown(data.spiralSlashCooldownSource, data.spiralSlashCooldown);
        usingSpiralSlash = true;
        spiralSlashHitList.Clear();

        weapon.SpiralSlashRpc();
        spiralSlashEndCoroutine = Timing.RunCoroutine(MyCoroutines.WaitThen(data.spiralSlashDuration, () => { usingSpiralSlash = false; }));
    }
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        weapon.circularSlashHitbox.onTriggerEnter2D += OnSpiralSlashTriggerEnter2D;
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        usingFlyingSlash = false;
        usingSpiralSlash = false;
        Timing.KillCoroutines(flyingSlashCoroutine);
        Timing.KillCoroutines(spiralSlashEndCoroutine);
        weapon.circularSlashHitbox.onTriggerEnter2D -= OnSpiralSlashTriggerEnter2D;
    }
    IEnumerator<float> FlyingSlash()
    {
        usingFlyingSlash = true;
        weapon.FlyingSlashRpc();
        yield return Timing.WaitForSeconds(data.flyingSlashDelay);

        IronSword_FlyingSlash flyingSlash = MonoBehaviour.Instantiate(data.flyingSlash, weapon.flyingSlashFirePoint.position, weapon.flyingSlashFirePoint.rotation);
        flyingSlash.Set(wielder, data.flyingSlashDamage, data.flyingSlashSpeed, data.flyingSlashAcceleration, data.flyingSlashRange, data.flyingSlashKnockback, data.flyingSlashSlowPercentage, data.flyingSlashSlowDuration);
        flyingSlash.NetworkObject.Spawn();

        yield return Timing.WaitForSeconds(data.flyingSlashDuration - data.flyingSlashDelay);
        usingFlyingSlash = false;
    }
    readonly List<GameObject> spiralSlashHitList = new();
    void OnSpiralSlashTriggerEnter2D(Collider2D other)
    {
        if (wielder == null) return;

        GameObject hit = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (spiralSlashHitList.Contains(hit)) return;
        spiralSlashHitList.Add(hit);

        OnSpiralSlashHit(hit);
    }
    void OnSpiralSlashHit(GameObject hit)
    {
        if(hit.TryGetComponent(out IReflectable reflectable))
        {
            if(wielder.GetRelation(reflectable.owner) == Relation.Enemy)
            {
                reflectable.Reflect(wielder, (hit.transform.position - wielder.transform.position).normalized);
                return;
            }
        }

        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if(wielder.GetRelation(damagable.owner) == Relation.Enemy)
            {
                damagable.GetDamage(new()
                {
                    amount = data.damage,
                    source = DamageSource.Player,
                    clientID = wielder.OwnerClientId
                });
            }
        }
        if (hit.TryGetComponent(out IKnockable knockable))
        {
            if(wielder.GetRelation(knockable.owner) == Relation.Enemy)
            {
                knockable.GetKnockback(new()
                {
                    value = (hit.transform.position - wielder.transform.position).normalized * data.spiralSlashKnockback,
                    source = KnockbackSource.Player,
                    clientID = wielder.OwnerClientId
                });
            }
        }
    }
}