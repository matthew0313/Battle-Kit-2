using System;
using Unity.Netcode;
using UnityEngine;

public class InventorySlot
{

    public readonly Inventory inventory;
    public readonly Func<Item, bool> slotRestriction;
    public InventorySlot(Inventory inventory = null, Func<Item, bool> slotRestriction = null)
    {
        this.inventory = inventory;
        this.slotRestriction = slotRestriction;
    }

    Item m_item;
    public Item item
    {
        get => m_item;
        set
        {
            if (m_item == value) return;
            Item prev = m_item;
            m_item = value;
            onItemChange?.Invoke(prev, m_item);
        }
    }
    public Action<Item, Item> onItemChange;
    int m_count = 0;
    public int count
    {
        get => m_count;
        set
        {
            if (m_count == value) return;
            m_count = value;
            if(m_count == 0)
            {
                item.Delete();
                item = null;
            }
            onCountChange?.Invoke();
        }
    }
    public Action onCountChange;
    public int Insert(Item item, int count)
    {
        if (this.item == null)
        {
            if (SetItem(item))
            {
                this.count = Mathf.Min(item.data.maxStack, this.count + count);
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
        if (this.item == null)
        {
            if (CanSetItem(item))
            {
                count -= Mathf.Min(item.data.maxStack, this.count + count);
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
        ForcedSetItem(item);
        return true;
    }
    public bool Swap(InventorySlot slot)
    {
        if (!slot.CanSetItem(item) || !CanSetItem(slot.item)) return false;
        Item tmp = item;
        int tmp2 = count;
        ForcedSetItem(slot.item);
        count = slot.count;
        slot.ForcedSetItem(tmp);
        slot.count = tmp2;
        return true;
    }
    public void ForcedSetItem(Item item)
    {
        if(this.item != null) this.item.containedSlot = null;
        this.item = item;
        if(this.item != null) this.item.containedSlot = this;
    }
}