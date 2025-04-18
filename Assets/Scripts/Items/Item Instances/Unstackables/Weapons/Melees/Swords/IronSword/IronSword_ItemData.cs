using System;
using System.Collections.Generic;
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
    [SerializeField] float m_flyingSlashDamage, m_flyingSlashSpeed, m_flyingSlashRange, m_flyingSlashAcceleration, m_flyingSlashCooldown;
    public Sprite flyingSlashSprite => m_flyingSlashSprite;
    public IronSword_FlyingSlash flyingSlash => m_flyingSlash;
    public float flyingSlashDamage => m_flyingSlashDamage;
    public float flyingSlashSpeed => m_flyingSlashSpeed;
    public float flyingSlashAcceleration => m_flyingSlashAcceleration;
    public float flyingSlashRange => m_flyingSlashRange;
    public float flyingSlashCooldown => m_flyingSlashCooldown;
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
    public Sprite skill1Sprite => data.flyingSlashSprite;
    public bool canUseSkill1 => flyingSlashCounter <= 0.0f && counter >= data.slashRate;
    public float skill1CooldownBarFill => flyingSlashCounter / data.flyingSlashCooldown;
    public void Skill1Use(Player wielder)
    {
        counter = 0.0f;
        flyingSlashCounter = data.flyingSlashCooldown;

        weapon.FlyingSlashRpc();

        IronSword_FlyingSlash flyingSlash = MonoBehaviour.Instantiate(data.flyingSlash, weapon.flyingSlashFirePoint.position, weapon.flyingSlashFirePoint.rotation);
        flyingSlash.Set(wielder, data.flyingSlashDamage, data.flyingSlashSpeed, data.flyingSlashAcceleration, data.flyingSlashRange);
        flyingSlash.NetworkObject.Spawn();
    }
    public override void Update()
    {
        base.Update();
        flyingSlashCounter = Mathf.Max(0.0f, flyingSlashCounter - Time.deltaTime);
    }
}