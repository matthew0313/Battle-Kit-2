using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GunItemData : WeaponItemData
{
    [Header("Gun")]
    [SerializeField] Sprite m_lmbIcon;
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage;
    [SerializeField] CooldownSource m_fireCooldownSource;
    [SerializeField] float m_fireCooldown, m_bulletSpeed, m_bulletRange, m_spread, m_reloadTime;
    [SerializeField] int m_shotCount = 1, m_magSize;
    [SerializeField] ItemData m_ammunition;
    [SerializeField] Sound m_fireSound, m_reloadSound;
    public Sprite lmbIcon => m_lmbIcon;
    public bool auto => m_auto;
    public float damage => m_damage;
    public CooldownSource fireCooldownSource => m_fireCooldownSource;
    public float fireCooldown => m_fireCooldown;
    public float bulletSpeed => m_bulletSpeed;
    public float bulletRange => m_bulletRange;
    public float spread => m_spread;
    public float reloadTime => m_reloadTime;
    public int shotCount => m_shotCount;
    public int magSize => m_magSize;
    public ItemData ammunition => m_ammunition;
    public Sound fireSound => m_fireSound;
    public Sound reloadSound => m_reloadSound;
    public abstract Bullet bullet { get; }

}
public abstract class GunItem : WeaponItem, IAmmoCountDisplayed, IReloadable, ILMBUser
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
    PlayingSound reloadingSound;
    public void Reload(Player wielder)
    {
        if (!canReload) return;
        reloadCounter = data.reloadTime;
        reloading = true;
        weapon.Reload();
        reloadingSound = NetworkAudioManager.Instance.PlaySound(data.fireSound);
    }
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        if(reloading)
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
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        NetworkAudioManager.Instance.StopSound(reloadingSound);
    }
    protected float fireCounter => data.fireCooldownSource.cooldownLeft;
    protected float fireCooldown => data.fireCooldownSource.cooldown;
    protected virtual bool canFire => true;
    public Sprite lmbSprite => data.lmbIcon;
    public bool canUseLmb => canFire && !reloading && fireCounter <= 0.0f && mag > 0;
    public float lmbCooldownBarFill => fireCounter / fireCooldown;
    public void LMBUse(Player wielder, bool down)
    {
        if (!data.auto && !down || !canUseLmb) return;
        wielder.cooldowns.AddCooldown(data.fireCooldownSource, data.fireCooldown);
        mag--;
        weapon.Fire();
        NetworkAudioManager.Instance.PlaySound(data.fireSound);
        Fire(wielder);
    }
    protected virtual void Fire(Player wielder)
    {
        for(int i = 0; i < data.shotCount; i++)
        {
            Bullet bullet = MonoBehaviour.Instantiate(data.bullet, weapon.firePoint.position, weapon.firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.spread, data.spread)));
            bullet.Set(data.damage, data.bulletSpeed, data.bulletRange);
            bullet.NetworkObject.Spawn();
        }
    }
    public override Dictionary<string, string> SaveProperties()
    {
        var data = base.SaveProperties();
        data.Add("Mag", mag.ToString());
        return data;
    }
    public override void LoadProperties(Dictionary<string, string> data)
    {
        base.LoadProperties(data);
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