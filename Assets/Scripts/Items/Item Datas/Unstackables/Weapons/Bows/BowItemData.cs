using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BowItemData : WeaponItemData
{
    [Header("Bow")]
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage, m_fireRate, m_arrowSpeed, m_arrowRange, m_spread;
    [SerializeField] ItemData m_ammunition;
    [SerializeField] Bullet m_bullet;
    public bool auto => m_auto;
    public float damage => m_damage;
    public float fireRate => m_fireRate;
    public float arrowSpeed => m_arrowSpeed;
    public float arrowRange => m_arrowRange;
    public float spread => m_spread;
    public ItemData ammunition => m_ammunition;
    public Bullet bullet => m_bullet;
}
public abstract class BowItem : WeaponItem, IAmmoCountDisplayed
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
    protected readonly ControlElement<Player_Movements> mouseLockedEnable = new ControlElement<Player_Movements>(0, (movements) => movements.mouseLocked = true, true);
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        counter = 0.0f;
        wielder.movements.mouseLockedControl.AddControls(mouseLockedEnable);
    }
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        counter += Time.deltaTime;
        if (counter >= data.fireRate)
        {
            if ((data.auto ? InputManager.Instance.ItemUseHoldInput() : InputManager.Instance.ItemUseInput()) && GetAmmoCount() > 0)
            {
                counter = 0.0f;
                containedSlot.inventory.TakeOut(ammunition, 1);
                Fire(wielder);
            }
        }
    }
    protected virtual void Fire(Player wielder)
    {
        weapon.FireRpc();
        Bullet bul = MonoBehaviour.Instantiate(data.bullet, weapon.firePoint.position, weapon.firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.spread, data.spread)));
        bul.Set(wielder, data.damage, data.arrowSpeed, data.arrowRange);
        bul.NetworkObject.Spawn();
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.movements.mouseLockedControl.RemoveControl(mouseLockedEnable);
    }
}