using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
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
    protected Player wielder { get; private set; }
    public virtual void OnWield(Player wielder) { this.wielder = wielder; }
    public virtual void OnWieldUpdate(Player wielder) { }
    public virtual void OnUnwield(Player wielder) { this.wielder = null; }

    //Instance
    public virtual Item Copy() => data.Create();
    public virtual void Delete()
    {
        ItemManager.Instance.RemoveItem(this);
    }

    //Saving
    public virtual Dictionary<string, string> SaveProperties() => new();
    public virtual void LoadProperties(Dictionary<string, string> data) { }
    public ItemInstanceData ToInstanceData()
    {
        return new()
        {
            itemIndex = ItemManager.Instance.itemDatabase.GetIndex(data),
            properties = (NetworkDictionary)SaveProperties()
        };
    }
}