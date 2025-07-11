using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class WeaponItemData : UnstackableItemData
{
    public abstract Weapon weaponPrefab { get; }
}
public abstract class WeaponItem : UnstackableItem
{
    new public readonly WeaponItemData data;
    protected Weapon weapon { get; private set; }
    public WeaponItem(WeaponItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        if(weapon == null)
        {
            weapon = MonoBehaviour.Instantiate(data.weaponPrefab);
            weapon.NetworkObject.Spawn();
            weapon.transform.position = wielder.inventory.weaponAnchor.position;
            weapon.transform.rotation = wielder.inventory.weaponAnchor.rotation;
        }
        weapon.enabledState.Value = true;
    }
    protected virtual bool scaleBound => true;
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        weapon.transform.position = wielder.inventory.weaponAnchor.position;
        if (scaleBound) weapon.transform.localScale = wielder.inventory.weaponAnchor.lossyScale;
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        weapon.enabledState.Value = false;
    }
    public override void Delete()
    {
        base.Delete();
        if(weapon != null) weapon.NetworkObject.Despawn();
    }
}