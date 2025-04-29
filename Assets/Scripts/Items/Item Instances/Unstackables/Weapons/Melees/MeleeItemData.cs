using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class MeleeItemData : WeaponItemData
{
    [Header("Melee")]
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage;
    [SerializeField] float m_slashRate;
    public bool auto => m_auto;
    public float damage => m_damage;
    public float slashRate => m_slashRate;
}
public abstract class MeleeItem : WeaponItem
{
    new public readonly MeleeItemData data;
    new protected Melee weapon => base.weapon as Melee;
    protected MeleeItem(MeleeItemData data) : base(data)
    {
        this.data = data;
        scaleBound = false;
    }
    protected float counter = 0.0f;
    protected readonly ControlElement<Player_Movements> mouseLockedEnable = new ControlElement<Player_Movements>(0, (movements) => movements.mouseLocked = true, true);
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        weapon.hitbox.onTriggerEnter2D += OnTriggerEnter2D;
        wielder.movements.mouseLockedControl.AddControls(mouseLockedEnable);
    }
    protected virtual bool canSlash => true;
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        counter += Time.deltaTime;
        if(counter >= data.slashRate)
        {
            if(canSlash && (data.auto ? InputManager.Instance.ItemUseHoldInput() : InputManager.Instance.ItemUseInput()))
            {
                counter = 0.0f;
                Slash(wielder);
            }
        }
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        weapon.hitbox.onTriggerEnter2D -= OnTriggerEnter2D;
        wielder.movements.mouseLockedControl.RemoveControl(mouseLockedEnable);
    }
    protected virtual void Slash(Player wielder)
    {
        hitList.Clear();
        weapon.SlashRpc();
    }
    protected List<GameObject> hitList = new();
    void OnTriggerEnter2D(Collider2D other)
    {
        if (wielder == null) return;

        GameObject hit = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (hitList.Contains(hit)) return;
        hitList.Add(hit);

        OnHit(hit);
    }
    protected virtual void OnHit(GameObject hit)
    {
        if (hit.TryGetComponent(out IDamagable damagable))
        {
            if (damagable.IsEnemy(wielder))
            {
                damagable.GetDamage(new()
                {
                    amount = data.damage,
                    damageSource = DamageSource.Player,
                    damageGiverClientID = wielder.OwnerClientId
                });
            }
        }
    }
}