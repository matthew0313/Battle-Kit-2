using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Glock", menuName = "Scriptables/Items/Unstackables/Weapons/Guns/Pistols/Glock")]
public class Glock_ItemData : PistolItemData
{
    [Header("Glock")]
    [SerializeField] Glock m_weaponPrefab;
    public override Weapon weaponPrefab => m_weaponPrefab;
    public override Item Create() => new Glock_Item(this);
}
public class Glock_Item : PistolItem
{
    new public readonly Glock_ItemData data;
    new protected Glock weapon => base.weapon as Glock;
    public Glock_Item(Glock_ItemData data) : base(data)
    {
        this.data = data;
    }
}