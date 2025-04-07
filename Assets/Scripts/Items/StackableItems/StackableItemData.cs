using Unity.Netcode;
using UnityEngine;

public abstract class StackableItemData : ItemData
{
    [Header("Stackable")]
    [SerializeField] int m_maxStack;
    public override int maxStack => m_maxStack;
}
public abstract class StackableItem : Item
{
    new public readonly StackableItemData data;
    public StackableItem(StackableItemData data) : base(data)
    {
        this.data = data;
    }
    //Stacking
    public override bool IsStackable(Item other) => other.data == data;
}