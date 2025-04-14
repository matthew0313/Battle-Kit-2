using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] LangText m_itemName;
    [SerializeField] Sprite m_itemIcon;
    public LangText itemName => m_itemName;
    public Sprite itemIcon => m_itemIcon;
    public abstract Item Create();
    public abstract int maxStack { get; }
}
public abstract class Item
{
    public readonly ItemData data;
    public Item(ItemData data)
    {
        this.data = data;
    }
    //Slot
    public InventorySlot containedSlot;

    //Stacking
    public abstract bool IsStackable(Item other);

    //Wielding
    public virtual void OnWield(Player wielder) { }
    public virtual void OnWieldUpdate(Player wielder) { }
    public virtual void OnUnwield(Player wielder) { }

    //Instance
    public virtual Item Copy() => data.Create();
    public virtual void Delete() { }

    //Saving
    public virtual Dictionary<string, string> Save() => new();
    public virtual void Load(Dictionary<string, string> data) { }
}