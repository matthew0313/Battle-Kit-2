using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    [SerializeField] LangText m_itemName;
    [SerializeField] LangText m_description;
    [SerializeField] Sprite m_itemIcon;
    public LangText itemName => m_itemName;
    public LangText description => m_description;
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
        ItemManager.Instance.AddItem(this);
    }
    //Slot
    InventorySlot m_containedSlot;
    public InventorySlot containedSlot
    {
        get => m_containedSlot;
        set
        {
            if (m_containedSlot == value) return;
            InventorySlot prev = m_containedSlot;
            m_containedSlot = value;
            OnSlotChange(prev, m_containedSlot);
        }
    }
    protected virtual void OnSlotChange(InventorySlot prevSlot, InventorySlot newSlot) { }

    //Stacking
    public abstract bool IsStackable(Item other);

    //Life Cycle
    public virtual void Update() { }

    //Wielding
    public virtual void OnWield(Player wielder) { }
    public virtual void OnWieldUpdate(Player wielder) { }
    public virtual void OnUnwield(Player wielder) { }

    //Instance
    public virtual Item Copy() => data.Create();
    public virtual void Delete()
    {
        ItemManager.Instance.RemoveItem(this);
    }

    //Saving
    public virtual Dictionary<string, string> Save() => new();
    public virtual void Load(Dictionary<string, string> data) { }
}