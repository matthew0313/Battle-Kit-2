using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BowItemData : WeaponItemData
{
    [Header("Bow")]
    [SerializeField] ItemData m_ammunition;
    [SerializeField] Bullet m_bullet;
    [SerializeField] float m_damage, m_fireRate, m_arrowSpeed, m_arrowRange;
    public ItemData ammunition => m_ammunition;
    public Bullet bullet => m_bullet;
    public float damage => m_damage;
    public float fireRate => m_fireRate;
    public float arrowSpeed => m_arrowSpeed;
    public float arrowRange => m_arrowRange;
}
public abstract class BowItem : WeaponItem, IAmmoCountDisplayed
{
    new public readonly BowItemData data;
    protected Bow bow => base.weapon as Bow;

    //Stats
    protected virtual ItemData ammunition => data.ammunition;

    //IAmmoCountDisplayed
    public Action onAmmoCountDisplayUpdate { get; set; }
    public string ammoCountText => $"{GetAmmoCount()}";
    public Sprite ammoIcon => ammunition.itemIcon;

    protected BowItem(BowItemData data) : base(data)
    {
        this.data = data;
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
            if (InputManager.Instance.ItemUseInput() && GetAmmoCount() > 0)
            {
                counter = 0.0f;
                containedSlot.inventory.TakeOut(ammunition, 1);
                Fire(wielder);
            }
        }
    }
    protected virtual void Fire(Player wielder)
    {
        bow.FireRpc();
        Bullet bul = MonoBehaviour.Instantiate(data.bullet, bow.firePoint.position, bow.firePoint.rotation);
        bul.Set(GetOnHit(wielder), data.arrowSpeed, data.arrowRange);
        bul.NetworkObject.Spawn();
    }
    protected virtual Action<Bullet, Collider2D> GetOnHit(Player wielder)
    {
        return (bullet, hit) =>
        {
            if (hit.TryGetComponent(out IDamagable damagable))
            {
                if (wielder.IsEnemy(Player.GetPlayerWithID(damagable.ownerID)))
                {
                    damagable.GetDamage(new()
                    {
                        amount = data.damage,
                        damageSource = DamageSource.Player,
                        damageGiverClientID = wielder.OwnerClientId
                    });
                    bullet.NetworkObject.Despawn(); return;
                }
            }

            if (hit.gameObject.layer == LayerMask.NameToLayer("Map")) bullet.NetworkObject.Despawn();
        };
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        wielder.movements.mouseLockedControl.RemoveControl(mouseLockedEnable);
    }
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
}