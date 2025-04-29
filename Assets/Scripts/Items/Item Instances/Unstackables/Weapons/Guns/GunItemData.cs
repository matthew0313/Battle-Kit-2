using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GunItemData : WeaponItemData
{
    [Header("Gun")]
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage, m_fireRate, m_bulletSpeed, m_bulletRange, m_spread, m_reloadTime;
    [SerializeField] int m_magSize;
    [SerializeField] ItemData m_ammunition;
    public bool auto => m_auto;
    public float damage => m_damage;
    public float fireRate => m_fireRate;
    public float bulletSpeed => m_bulletSpeed;
    public float bulletRange => m_bulletRange;
    public float spread => m_spread;
    public float reloadTime => m_reloadTime;
    public int magSize => m_magSize;
    public ItemData ammunition => m_ammunition;
    public abstract Bullet bullet { get; }

}
public abstract class GunItem : WeaponItem, IAmmoCountDisplayed, IReloadable
{
    new public readonly GunItemData data;
    new protected Gun weapon => base.weapon as Gun;
    protected GunItem(GunItemData data) : base(data)
    {
        this.data = data;
        this.mag = data.magSize;
    }
    protected virtual ItemData ammunition => data.ammunition;

    //AmmoCount
    public Action onAmmoCountDisplayUpdate { get; set; }
    public string ammoCountText => $"{mag}/{GetAmmoCount()}";
    public Sprite ammoIcon => data.ammunition.itemIcon;
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
        if (item != null && item.data == data.ammunition) onAmmoCountDisplayUpdate?.Invoke();
    }
    protected int GetAmmoCount()
    {
        return (containedSlot != null && containedSlot.inventory != null) ? containedSlot.inventory.Search(data.ammunition) : 0;
    }

    //Reloadable
    int m_mag = 0;
    protected int mag
    {
        get => m_mag;
        set
        {
            if (m_mag == value) return;
            m_mag = value;
            onAmmoCountDisplayUpdate?.Invoke();
        }
    }

    protected bool reloading = false;
    protected float reloadCounter = 0.0f;
    public bool canReload => !reloading && mag < data.magSize && GetAmmoCount() > 0;
    public float reloadBarFill => reloadCounter / data.reloadTime;
    public void Reload(Player wielder)
    {
        if (!canReload) return;
        reloadCounter = data.reloadTime;
        reloading = true;
        weapon.ReloadRpc();
    }

    protected float counter = 0.0f;
    protected readonly ControlElement<Player_Movements> mouseLockedEnable = new ControlElement<Player_Movements>(0, (movements) => movements.mouseLocked = true, true);
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        counter = 0.0f;
        wielder.movements.mouseLockedControl.AddControls(mouseLockedEnable);
    }
    protected virtual bool canFire => true;
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        counter += Time.deltaTime;
        if (!reloading)
        {
            if (counter >= data.fireRate)
            {
                if (canFire && (data.auto ? InputManager.Instance.ItemUseHoldInput() : InputManager.Instance.ItemUseInput()) && mag > 0)
                {
                    counter = 0.0f;
                    mag--;
                    weapon.FireRpc();
                    Fire(wielder);
                }
            }
        }
        else
        {
            reloadCounter = Mathf.Max(0.0f, reloadCounter - Time.deltaTime);
            if(reloadCounter <= 0.0f)
            {
                reloading = false;
                int amount = Mathf.Min(data.magSize - mag, GetAmmoCount());
                mag += amount;
                containedSlot.inventory.TakeOut(ammunition, amount);
            }
        }
    }
    protected virtual void Fire(Player wielder)
    {
        Bullet bullet = MonoBehaviour.Instantiate(data.bullet, weapon.firePoint.position, weapon.firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.spread, data.spread)));
        bullet.Set(wielder, data.damage, data.bulletSpeed, data.bulletRange);
        bullet.NetworkObject.Spawn();
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.movements.mouseLockedControl.RemoveControl(mouseLockedEnable);
    }
    public override Dictionary<string, string> Save()
    {
        var data = base.Save();
        data.Add("Mag", mag.ToString());
        return data;
    }
    public override void Load(Dictionary<string, string> data)
    {
        base.Load(data);
        if (data == null) return;
        if (data.TryGetValue("Mag", out string mag)) this.mag = int.Parse(mag);
    }
    public override Item Copy()
    {
        GunItem tmp = base.Copy() as GunItem;
        tmp.mag = mag;
        return tmp;
    }
}