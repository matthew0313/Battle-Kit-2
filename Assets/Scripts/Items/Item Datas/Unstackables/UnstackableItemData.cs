using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Unstackable Item Data", menuName = "Scriptables/Items/Unstackables/Item")]
public class UnstackableItemData : ItemData
{
    public override int maxStack => 1;
    public override Item Create() => new UnstackableItem(this);
}
public class UnstackableItem : Item
{
    new public readonly UnstackableItemData data;
    public UnstackableItem(UnstackableItemData data) : base(data)
    {
        this.data = data;
    }
    public override bool IsStackable(Item other) => false;
}