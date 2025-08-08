using System;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "FlameSword", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Swords/FlameSword")]
public class FlameSword_ItemData : SwordItemData
{
    public override Item Create() => new FlameSword_Item(this);
}
public class FlameSword_Item : SwordItem, ISkill1User, ISkill2User
{
    new public readonly FlameSword_ItemData data;
    new protected FlameSword weapon => base.weapon as FlameSword;

    public FlameSword_Item(FlameSword_ItemData data) : base(data)
    {
        this.data = data;
    }
}