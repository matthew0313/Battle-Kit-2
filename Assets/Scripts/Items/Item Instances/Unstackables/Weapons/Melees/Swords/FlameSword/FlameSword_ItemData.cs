using System;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "FlameSword", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Swords/FlameSword")]
public class FlameSword_ItemData : SwordItemData
{
    [Header("FlameSword")]
    [SerializeField] FlameSword m_weaponPrefab;
    [SerializeField] float m_burnDuration, m_burnDamage;
    public override Weapon weaponPrefab => m_weaponPrefab;
    public float burnDuration => m_burnDuration;
    public float burnDamage => m_burnDamage;

    [Header("FlameBlessing")]
    [SerializeField] Sprite m_flameBlessingSprite;
    [SerializeField] float m_flameBlessingDuration, m_flameBlessingDamage, m_flameBlessingAttackMultiplier, m_flameBlessingDefenseMultiplier;
    [SerializeField] CooldownSource m_flameBlessingCooldownSource;
    [SerializeField] float m_flameBlessingCooldown;
    [SerializeField] UnitTime m_flameBlessingStartup, m_flameBlessingCastDuration;
    public Sprite flameBlessingSprite => m_flameBlessingSprite;
    public float flameBlessingDuration => m_flameBlessingDuration;
    public float flameBlessingDamage => m_flameBlessingDamage;
    public float flameBlessingAttackMultiplier => m_flameBlessingAttackMultiplier;
    public float flameBlessingDefenseMultiplier => m_flameBlessingDefenseMultiplier;
    public CooldownSource flameBlessingCooldownSource => m_flameBlessingCooldownSource;
    public float flameBlessingCooldown => m_flameBlessingCooldown;
    public UnitTime flameBlessingStartup => m_flameBlessingStartup;
    public UnitTime flameBlessingCastDuration => m_flameBlessingCastDuration;
    public override Item Create() => new FlameSword_Item(this);
}
public class FlameSword_Item : SwordItem, ISkill1User
{
    new public readonly FlameSword_ItemData data;
    new protected FlameSword weapon => base.weapon as FlameSword;

    public FlameSword_Item(FlameSword_ItemData data) : base(data)
    {
        this.data = data;
    }
    protected override void OnHit(GameObject hit)
    {
        base.OnHit(hit);
        if(hit.TryGetComponent(out IAlimentable alimentable))
        {
            if(wielder.GetRelation(alimentable.owner) == Relation.Enemy)
            {
                alimentable.AddAliment(new Burning_Aliment(data.burnDuration, data.burnDamage));
            }
        }
    }
    float flameBlessingCounter => data.flameBlessingCooldownSource.cooldownLeft;
    float flameBlessingCooldown => data.flameBlessingCooldownSource.cooldown;
    bool usingFlameBlessing = false;
    public Sprite skill1Sprite => data.flameBlessingSprite;
    public bool canUseSkill1 => flameBlessingCounter <= 0.0f;
    public float skill1CooldownBarFill => flameBlessingCounter / flameBlessingCooldown;
    CoroutineHandle flameBlessingCoroutine;
    protected override bool canSlash => base.canSlash && !usingFlameBlessing;
    public void Skill1Use(Player wielder)
    {
        if (!canUseSkill1) return;
        wielder.cooldowns.AddCooldown(data.flameBlessingCooldownSource, data.flameBlessingCooldown);
        weapon.FlameBlessing();
        flameBlessingCoroutine = Timing.RunCoroutine(FlameBlessing(wielder));
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        Timing.KillCoroutines(flameBlessingCoroutine);
        usingFlameBlessing = false;
    }
    IEnumerator<float> FlameBlessing(Player wielder)
    {
        usingFlameBlessing = true;
        yield return Timing.WaitForSeconds(data.flameBlessingStartup);
        wielder.hpComp.AddAliment(new Burning_Aliment(data.flameBlessingDuration, data.flameBlessingDamage));
        wielder.hpComp.AddAliment(new AttackBuff_Aliment(data.flameBlessingDuration, data.flameBlessingAttackMultiplier));
        wielder.hpComp.AddAliment(new DefenseBuff_Aliment(data.flameBlessingDuration, data.flameBlessingDefenseMultiplier));
        yield return Timing.WaitForSeconds(data.flameBlessingCastDuration - data.flameBlessingStartup);
        usingFlameBlessing = false;
    }
}