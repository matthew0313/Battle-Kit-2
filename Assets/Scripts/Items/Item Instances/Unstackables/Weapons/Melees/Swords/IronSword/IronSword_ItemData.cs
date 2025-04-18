using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "IronSword", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Swords/IronSword")]
public class IronSword_ItemData : SwordItemData
{
    [Header("IronSword")]
    [SerializeField] IronSword m_weaponPrefab;
    public override Weapon weaponPrefab => m_weaponPrefab;
    public override Item Create() => new IronSword_Item(this);
}
public class IronSword_Item : SwordItem
{
    new public readonly IronSword_ItemData data;
    new protected IronSword weapon => base.weapon as IronSword;
    public IronSword_Item(IronSword_ItemData data) : base(data)
    {
        this.data = data;
    }
}