using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunItemData : WeaponItemData
{
    [Header("Gun")]
    [SerializeField] ItemData m_ammunition;
    [SerializeField] int m_magSize;
    public ItemData ammunition => m_ammunition;
}
public abstract class GunItem : WeaponItem, IAmmoCountDisplayed
{
    new public readonly GunItemData data;
    protected Gun gun => base.weapon as Gun;

    //IAmmoCountDisplayed
    public Action onAmmoCountDisplayUpdate { get; set; }
    public string ammoCountText => $"{mag}/{GetAmmoCount()}";
    public Sprite ammoIcon => data.ammunition.itemIcon;
    protected int mag = 0;

    protected GunItem(GunItemData data) : base(data)
    {
        this.data = data;
    }
    protected float counter = 0.0f;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        counter = 0.0f;
        wielder.movements.mouseLocked = true;
    }
    protected override void OnSlotChange(InventorySlot prevSlot, InventorySlot newSlot)
    {
        base.OnSlotChange(prevSlot, newSlot);
        if(prevSlot != null)
        {
            prevSlot.inventory.onInventoryUpdate -= OnInventoryUpdate;
        }
        if(newSlot != null)
        {
            newSlot.inventory.onInventoryUpdate += OnInventoryUpdate;
        }
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.movements.mouseLocked = false;
    }
    void OnInventoryUpdate(Item item)
    {
        if (item != null && item.data == data.ammunition) onAmmoCountDisplayUpdate?.Invoke();
    }
    protected int GetAmmoCount()
    {
        return (containedSlot != null && containedSlot.inventory != null) ? containedSlot.inventory.Search(data.ammunition) : 0;
    }
}