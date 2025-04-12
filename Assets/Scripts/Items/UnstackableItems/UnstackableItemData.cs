using Unity.Netcode;
using UnityEngine;

public abstract class UnstackableItemData : ItemData
{
    public override int maxStack => 1;
}
public abstract class UnstackableItem : Item
{
    new public readonly UnstackableItemData data;
    protected UnstackableItem(UnstackableItemData data) : base(data)
    {
        this.data = data;
    }
    //Stacking
    public override bool IsStackable(Item other) => false;
}