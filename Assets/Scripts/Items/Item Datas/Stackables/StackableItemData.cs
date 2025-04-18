using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Stackable Item Data", menuName = "Scriptables/Items/Stackables/Item")]
public class StackableItemData : ItemData
{
    [Header("Stackable")]
    [SerializeField] int m_maxStack = 99;
    public override int maxStack => m_maxStack;
    public override Item Create() => new StackableItem(this);
}
public class StackableItem : Item
{
    new public readonly StackableItemData data;
    public StackableItem(StackableItemData data) : base(data)
    {
        this.data = data;
    }
    public override bool IsStackable(Item other) => other != null && other.data == data;
}