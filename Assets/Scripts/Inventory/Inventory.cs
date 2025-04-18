using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Jobs;

public class Inventory : NetworkBehaviour
{
    [SerializeField] protected int defaultSlotCount = 16;
    public List<InventorySlot> slots { get; } = new();
    public Action<Item> onInventoryUpdate;
    int m_slotCount = 0;
    public Action onSlotCountChange;
    public int slotCount
    {
        get { return m_slotCount; }
        set
        {
            for (int i = m_slotCount; i < value; i++)
            {
                InventorySlot tmp = new InventorySlot(this);
                tmp.onItemChange += (prevItem, newItem) => { onInventoryUpdate?.Invoke(prevItem); onInventoryUpdate?.Invoke(newItem); };
                tmp.onCountChange += () => { onInventoryUpdate?.Invoke(tmp.item); };
                slots.Add(tmp);
            }
            m_slotCount = value;
            onSlotCountChange?.Invoke();
        }
    }
    public void Init()
    {
        slotCount = defaultSlotCount;
    }
    [Rpc(SendTo.Owner)]
    public void InsertRpc(ItemInstanceData instanceData, int count, NetworkBehaviourReference source)
    {
        Item item = ItemManager.Instance.itemDatabase.GetItemData(instanceData.itemIndex).Create();
        item.Load(instanceData.data.RetrieveData());
        int left = Insert(item, count);
        if(source.TryGet(out NetworkBehaviour sender) && sender is IntegerReceiver receiver)
        {
            receiver.ResponseRpc(left);
        }
    }
    [Rpc(SendTo.Owner)]
    public void ClearRpc()
    {
        foreach (var i in slots) i.count = 0;
    }
    public int Insert(Item item, int count)
    {
        foreach (var i in slots)
        {
            if (i.item != null && i.item.IsStackable(item) && item.IsStackable(i.item))
            {
                count = i.Insert(item, count);
                if (count <= 0) return 0;
            }
        }
        for (int i = 0; i < slotCount; i++)
        {
            count = slots[i].Insert(item.Copy(), count);
            if (count <= 0) return 0;
        }
        onInventoryUpdate?.Invoke(item);
        return count;
    }
    public int TryInsert(Item item, int count)
    {
        foreach (var i in slots)
        {
            if (i.item != null && i.item.IsStackable(item) && item.IsStackable(i.item))
            {
                count = i.TryInsert(item, count);
                if (count <= 0) return 0;
            }
        }
        for (int i = 0; i < slotCount; i++)
        {
            count = slots[i].TryInsert(item.Copy(), count);
            if (count <= 0) return 0;
        }
        return count;
    }
    public int Search(ItemData data)
    {
        return Search((item) => item.data == data);
    }
    public int Search(Func<Item, bool> predicate)
    {
        int tot = 0;
        foreach (var i in slots)
        {
            if (i.item != null && predicate.Invoke(i.item))
            {
                tot += i.count;
            }
        }
        return tot;
    }
    public IEnumerable<InventorySlot> SearchAll(Func<Item, bool> predicate)
    {
        foreach (var i in slots)
        {
            if (i.item != null && predicate.Invoke(i.item)) yield return i;
        }
    }
    public bool TakeOut(ItemData data, int count)
    {
        return TakeOut((item) => item.data == data, count);
    }
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
    public override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var i in slots)
        {
            if (i.item != null) i.item.Delete();
        }
    }
}