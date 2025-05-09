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
    [SerializeField] float m_flyingSlashDelay = 0.5f, m_flyingSlashDuration = 1.0f,
        m_flyingSlashDamage, m_flyingSlashSpeed, m_flyingSlashRange, m_flyingSlashAcceleration, m_flyingSlashCooldown, m_flyingSlashKnockback,
        m_flyingSlashSlowPercentage, m_flyingSlashSlowDuration;
    public Sprite flyingSlashSprite => m_flyingSlashSprite;
    public IronSword_FlyingSlash flyingSlash => m_flyingSlash;
    public float flyingSlashDelay => m_flyingSlashDelay;
    public float flyingSlashDuration => m_flyingSlashDuration;
    public float flyingSlashDamage => m_flyingSlashDamage;
    public float flyingSlashSpeed => m_flyingSlashSpeed;
    public float flyingSlashAcceleration => m_flyingSlashAcceleration;
    public float flyingSlashRange => m_flyingSlashRange;
    public float flyingSlashCooldown => m_flyingSlashCooldown;
    public float flyingSlashKnockback => m_flyingSlashKnockback;
    public float flyingSlashSlowPercentage => m_flyingSlashSlowPercentage;
    public float flyingSlashSlowDuration => m_flyingSlashSlowDuration;
    public override Item Create() => new IronSword_Item(this);
}
public class IronSword_Item : SwordItem, ISkill1User
{
    new public readonly IronSword_ItemData data;
    new protected IronSword weapon => base.weapon as IronSword;

    public IronSword_Item(IronSword_ItemData data) : base(data)
    {
        this.data = data;
    }
    float flyingSlashCounter = 0.0f;
    bool usingFlyingSlash = false;
    public Sprite skill1Sprite => data.flyingSlashSprite;
    public bool canUseSkill1 => flyingSlashCounter <= 0.0f && counter >= data.slashRate;
    public float skill1CooldownBarFill => flyingSlashCounter / data.flyingSlashCooldown;
    CoroutineHandle flyingSlashCoroutine;
    protected override bool canSlash => base.canSlash && !usingFlyingSlash;
    public void Skill1Use(Player wielder)
    {
        flyingSlashCounter = data.flyingSlashCooldown;
        usingFlyingSlash = true;

        weapon.FlyingSlashRpc();
        flyingSlashCoroutine = Timing.RunCoroutine(FlyingSlash());
        Timing.RunCoroutine(MyCoroutines.WaitThen(data.flyingSlashDuration, () => usingFlyingSlash = false));
    }
    public override void Update()
    {
        base.Update();
        flyingSlashCounter = Mathf.Max(0.0f, flyingSlashCounter - Time.deltaTime);
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        Timing.KillCoroutines(flyingSlashCoroutine);
    }
    IEnumerator<float> FlyingSlash()
    {
        yield return Timing.WaitForSeconds(data.flyingSlashDelay);

        IronSword_FlyingSlash flyingSlash = MonoBehaviour.Instantiate(data.flyingSlash, weapon.flyingSlashFirePoint.position, weapon.flyingSlashFirePoint.rotation);
        flyingSlash.Set(wielder, data.flyingSlashDamage, data.flyingSlashSpeed, data.flyingSlashAcceleration, data.flyingSlashRange, data.flyingSlashKnockback, data.flyingSlashSlowPercentage, data.flyingSlashSlowDuration);
        flyingSlash.NetworkObject.Spawn();
    }
}