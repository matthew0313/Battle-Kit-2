using System;
using Unity.Netcode;
using UnityEngine;

public class InventorySlot
{

    readonly Func<Item, bool> slotRestriction;
    public readonly Inventory inventory;
    public InventorySlot(Inventory inventory) { this.inventory = inventory; }
    public InventorySlot(Inventory inventory, Func<Item, bool> slotRestriction) { this.inventory = inventory; this.slotRestriction = slotRestriction; }


    public event Action onItemChange, onCountChange;
    Item m_item = null;
    public Item item
    {
        get => m_item;
        set
        {
            m_item = value;
            if (m_item != null) m_item.containedSlot = this;
            onItemChange?.Invoke();
        }
    }
    int m_count = 0;
    public int count
    {
        get => m_count;
        set
        {
            m_count = value;
            onCountChange?.Invoke();
            if (count <= 0)
            {
                m_count = 0;
                if (item != null)
                {
                    item.Delete();
                    item.containedSlot = null;
                    item = null;
                }
            }
        }
    }
    public int Insert(Item item, int count)
    {
        if (count <= 0) return 0;
        if (this.item == null)
        {
            if (SetItem(item.Copy()))
            {
                this.count = Mathf.Min(item.data.maxStack, count);
                count -= this.count;
            }
        }
        else if (this.item.IsStackable(item) && item.IsStackable(this.item))
        {
            int prev = this.count;
            this.count = Mathf.Min(this.item.data.maxStack, this.count + count);
            count -= this.count - prev;
        }
        return count;
    }
    public int TryInsert(Item item, int count)
    {
        if (count <= 0) return 0;
        if (this.item == null)
        {
            if (CanSetItem(item))
            {
                count -= Mathf.Min(item.data.maxStack, count);
            }
        }
        else if (this.item.IsStackable(item) && item.IsStackable(this.item))
        {
            count -= Mathf.Min(this.item.data.maxStack, this.count + count) - this.count;
        }
        return count;
    }
    public bool CanSetItem(Item item)
    {
        if (slotRestriction != null && slotRestriction.Invoke(item) == false) return false;
        return true;
    }
    public bool SetItem(Item item)
    {
        if (!CanSetItem(item)) return false;
        this.item = item;
        return true;
    }
    public bool Swap(InventorySlot slot)
    {
        if (!slot.CanSetItem(item) || !CanSetItem(slot.item)) return false;
        Item tmp = item;
        int tmp2 = count;

        item = slot.item;
        count = slot.count;

        slot.item = tmp;
        slot.count = tmp2;
        return true;
    }
}