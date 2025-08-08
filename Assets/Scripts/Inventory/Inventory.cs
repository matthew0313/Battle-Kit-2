using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Jobs;

public abstract class Inventory : NetworkBehaviour
{
    public List<InventorySlot> slots { get; } = new();
    public event Action<Item> onInventoryUpdate;
    int m_slotCount = 0;
    public event Action onSlotCountChange;
    public int slotCount
    {
        get { return m_slotCount; }
        protected set
        {
            m_slotCount = value;
            for (int i = slots.Count; i < slotCount; i++)
            {
                InventorySlot tmp = new InventorySlot(this);
                tmp.onItemChange += () => { if (tmp.item != null) onInventoryUpdate?.Invoke(tmp.item); };
                tmp.onCountChange += () => { onInventoryUpdate?.Invoke(tmp.item); };
                slots.Add(tmp);
            }
            onSlotCountChange?.Invoke();
        }
    }
    [Rpc(SendTo.Authority)]
    public void InsertRpc(ItemInstanceData data, int count, NetworkBehaviourReference source)
    {
        int left = Insert(data.ToItem(), count);
        if(source.TryGet(out IntegerReceiver receiver))
        {
            receiver.ResponseRpc(left);
        }
    }
    [Rpc(SendTo.Owner)]
    public void ClearRpc() => Clear();
    protected virtual IEnumerable<InventorySlot> GetInsertableSlots() => slots;
    protected virtual IEnumerable<InventorySlot> GetSearchableSlots() => slots;
    public int Insert(Item item, int count)
    {
        foreach (var i in GetInsertableSlots())
        {
            if (i.item != null && i.item.IsStackable(item) && item.IsStackable(i.item))
            {
                count = i.Insert(item, count);
                if (count <= 0) return 0;
            }
        }
        foreach (var i in GetInsertableSlots())
        {
            count = i.Insert(item, count);
            if (count <= 0) return 0;
        }
        return count;
    }
    public int TryInsert(Item item, int count)
    {
        foreach (var i in GetInsertableSlots())
        {
            if (i.item != null && i.item.IsStackable(item) && item.IsStackable(i.item))
            {
                count = i.TryInsert(item, count);
                if (count <= 0) return 0;
            }
        }
        foreach (var i in GetInsertableSlots())
        {
            count = i.TryInsert(item, count);
            if (count <= 0) return 0;
        }
        return count;
    }
    public IEnumerable<InventorySlot> SearchAll(Func<Item, bool> predicate)
    {
        foreach (var i in GetSearchableSlots())
        {
            if (i.item != null && predicate.Invoke(i.item)) yield return i;
        }
    }
    public int Search(Func<Item, bool> predicate)
    {
        int tot = 0;
        foreach (var i in SearchAll(predicate)) tot += i.count;
        return tot;
    }
    public int Search(ItemData data) => Search((item) => item != null && item.data == data);
    public bool TakeOut(Func<Item, bool> predicate, int count)
    {
        if (Search(predicate) < count) return false;
        foreach (var i in SearchAll(predicate))
        {
            Item tmp = i.item;
            int prev = count;
            count = Mathf.Max(0, count - i.count);
            i.count -= prev - count;
            if (count <= 0) break;
        }
        return true;
    }
    public bool TakeOut(ItemData data, int count) => TakeOut((item) => item.data == data, count);
    protected virtual void Clear()
    {
        foreach (var i in slots) i.count = 0;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var i in GetSearchableSlots())
        {
            if (i.item != null) i.item.Delete();
        }
    }
}