using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using NetworkAudio;
public abstract class BowItemData : WeaponItemData
{
    [Header("Bow")]
    [SerializeField] Sprite m_lmbIcon;
    [SerializeField] float m_damage;
    [SerializeField] CooldownSource m_fireCooldownSource;
    [SerializeField] float m_fireCooldown, m_arrowLifetime, m_arrowVelocity, m_spread;
    [SerializeField] ItemData m_ammunition;
    [SerializeField] Sound m_fireSound;
    public Sprite lmbIcon => m_lmbIcon;
    public float damage => m_damage;
    public CooldownSource fireCooldownSource => m_fireCooldownSource;
    public float fireCooldown => m_fireCooldown;
    public float arrowLifetime => m_arrowLifetime;
    public float arrowVelocity => m_arrowVelocity;
    public float spread => m_spread;
    public ItemData ammunition => m_ammunition;
    public Sound fireSound => m_fireSound;
    public abstract Arrow arrow { get; }
}
public abstract class BowItem : WeaponItem, IAmmoCountDisplayed, ILMBUser
{
    new public readonly BowItemData data;
    new protected Bow weapon => base.weapon as Bow;
    protected BowItem(BowItemData data) : base(data)
    {
        this.data = data;
    }
    protected virtual ItemData ammunition => data.ammunition;

    //AmmoCount
    public Action onAmmoCountDisplayUpdate { get; set; }
    public string ammoCountText => $"{GetAmmoCount()}";
    public Sprite ammoIcon => ammunition.itemIcon;
    protected override void OnSlotChange(InventorySlot prevSlot, InventorySlot newSlot)
    {
        base.OnSlotChange(prevSlot, newSlot);
        if (prevSlot != null)
        {
            prevSlot.inventory.onInventoryUpdate -= OnInventoryUpdate;
        }
        if (newSlot != null)
        {
            newSlot.inventory.onInventoryUpdate += OnInventoryUpdate;
        }
    }
    void OnInventoryUpdate(Item item)
    {
        if (item != null && item.data == ammunition) onAmmoCountDisplayUpdate?.Invoke();
    }
    protected int GetAmmoCount()
    {
        return (containedSlot != null && containedSlot.inventory != null) ? containedSlot.inventory.Search(ammunition) : 0;
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
    protected float fireCounter => data.fireCooldownSource.cooldownLeft;
    protected float fireCooldown => data.fireCooldownSource.cooldown;
    protected virtual bool canFire => true;
    public Sprite lmbSprite => data.lmbIcon;
    public bool canUseLmb => canFire && fireCounter <= 0.0f && GetAmmoCount() > 0;
    public float lmbCooldownBarFill => fireCounter / fireCooldown;
    public void LMBUse(Player wielder, bool down)
    {
        if (!down || !canUseLmb) return;
        wielder.cooldowns.AddCooldown(data.fireCooldownSource, data.fireCooldown);
        containedSlot.inventory.TakeOut(ammunition, 1);
        weapon.Fire();
        NetworkAudioManager.Instance.PlaySound(data.fireSound);
        Fire(wielder);
    }
    protected virtual void Fire(Player wielder)
    {
        Arrow arrow = MonoBehaviour.Instantiate(data.arrow, weapon.firePoint.position, weapon.firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.spread, data.spread)));
        arrow.Set(data.damage, data.arrowLifetime, data.arrowVelocity);
        arrow.NetworkObject.Spawn();
    }
}