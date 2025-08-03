using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BowItemData : WeaponItemData
{
    [Header("Bow")]
    [SerializeField] Sprite m_lmbIcon;
    [SerializeField] float m_damage, m_fireRate, m_arrowLifetime, m_arrowVelocity, m_spread;
    [SerializeField] ItemData m_ammunition;
    public Sprite lmbIcon => m_lmbIcon;
    public float damage => m_damage;
    public float fireRate => m_fireRate;
    public float arrowLifetime => m_arrowLifetime;
    public float arrowVelocity => m_arrowVelocity;
    public float spread => m_spread;
    public ItemData ammunition => m_ammunition;
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
    protected virtual bool canFire => true;
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        counter += Time.deltaTime;
    }
    public Sprite lmbSprite => data.lmbIcon;
    public bool canUseLmb => canFire && counter >= data.fireRate && GetAmmoCount() > 0;
    public float lmbCooldownBarFill => 1.0f - counter / data.fireRate;
    public void LMBUse(Player wielder, bool down)
    {
        if (!down) return;
        if (canFire && counter >= data.fireRate && GetAmmoCount() > 0)
        {
            counter = 0.0f;
            containedSlot.inventory.TakeOut(ammunition, 1);
            weapon.FireRpc();
            Fire(wielder);
        }
    }
    protected virtual void Fire(Player wielder)
    {
        Arrow arrow = MonoBehaviour.Instantiate(data.arrow, weapon.firePoint.position, weapon.firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.spread, data.spread)));
        arrow.Set(data.damage, data.arrowLifetime, data.arrowVelocity);
        arrow.NetworkObject.Spawn();
    }
}