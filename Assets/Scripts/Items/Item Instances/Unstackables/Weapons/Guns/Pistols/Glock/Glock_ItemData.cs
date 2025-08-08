using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Glock", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/Glock")]
public class Glock_ItemData : PistolItemData
{
    [Header("Glock")]
    [SerializeField] Glock m_weaponPrefab;
    [SerializeField] Glock_Bullet m_bullet;
    public override Weapon weaponPrefab => m_weaponPrefab;
    public override Bullet bullet => m_bullet;

    [Header("Shotgun Shot")]
    [SerializeField] Sprite m_shotgunShotIcon;
    [SerializeField] Glock_Bullet m_shotgunShotBullet;
    [SerializeField] int m_shotgunShotCount;
    [SerializeField] float m_shotgunShotDamage, m_shotgunShotSpeed, m_shotgunShotRange, m_shotgunShotSpread;
    [SerializeField] CooldownSource m_shotgunShotCooldownSource;
    [SerializeField] float m_shotgunShotCooldown;
    public Sprite shotgunShotIcon => m_shotgunShotIcon;
    public Glock_Bullet shotgunShotBullet => m_shotgunShotBullet;
    public int shotgunShotCount => m_shotgunShotCount;
    public float shotgunShotDamage => m_shotgunShotDamage;
    public float shotgunShotSpeed => m_shotgunShotSpeed;
    public float shotgunShotRange => m_shotgunShotRange;
    public float shotgunShotSpread => m_shotgunShotSpread;
    public CooldownSource shotgunShotCooldownSource => m_shotgunShotCooldownSource;
    public float shotgunShotCooldown => m_shotgunShotCooldown;
    public override Item Create() => new Glock_Item(this);
}
public class Glock_Item : PistolItem, ISkill1User
{
    new public readonly Glock_ItemData data;
    new protected Glock weapon => base.weapon as Glock;

    public Glock_Item(Glock_ItemData data) : base(data)
    {
        this.data = data;
    }
    float shotgunShotCounter => data.shotgunShotCooldownSource.cooldown;
    float shotgunShotCooldown => data.shotgunShotCooldownSource.cooldown;
    public Sprite skill1Sprite => data.shotgunShotIcon;
    public bool canUseSkill1 => mag >= data.shotgunShotCount && fireCounter <= 0.0f && shotgunShotCounter <= 0.0f;
    public float skill1CooldownBarFill => shotgunShotCounter / shotgunShotCooldown;
    public void Skill1Use(Player wielder)
    {
        if (!canUseSkill1) return;
        wielder.cooldowns.AddCooldown(data.shotgunShotCooldownSource, data.shotgunShotCooldown);
        mag -= data.shotgunShotCount;
        weapon.FireRpc();
        for (int i = 0; i < data.shotgunShotCount; i++)
        {
            Bullet bullet = MonoBehaviour.Instantiate(data.shotgunShotBullet, weapon.firePoint.position, weapon.firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.shotgunShotSpread, data.shotgunShotSpread)));
            bullet.transform.localScale = weapon.firePoint.lossyScale;
            bullet.Set(data.shotgunShotDamage, data.shotgunShotSpeed, data.shotgunShotRange);
            bullet.NetworkObject.Spawn();
        }
    }
}