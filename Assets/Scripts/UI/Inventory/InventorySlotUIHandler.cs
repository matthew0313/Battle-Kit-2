using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUIHandler : MonoBehaviour, ILocalPlayerSpawnCallbackReceiver
{
    public static InventorySlotUIHandler Instance { get; private set; }
    public InventorySlotUIHandler() => Instance = this;

    Player player;

    [SerializeField] InventorySlotUI grabbingSlotUI;
    InventorySlot prevSlot;
    public InventorySlot grabbingSlot;
    public bool grabbing => grabbingSlot.item != null;
    public void OnLocalPlayerSpawn(Player player)
    {
        this.player = player;
        grabbingSlot = new(player.inventory);
        grabbingSlotUI.Set(grabbingSlot);
    }
    private void Update()
    {
        if (player == null) return;
        GrabUpdate();
    }
    bool SearchSlot(out InventorySlotUI slotUI)
    {
        List<RaycastResult> results = UIScanner.ScanUI(Input.mousePosition);
        foreach(var i in results)
        {
            if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
            {
                slotUI = slot;
                return true;
            }
        }
        slotUI = null; return false;
    }
    void GrabUpdate()
    {
        Vector2 pos = Input.mousePosition;
        if (DeviceManager.deviceType == DeviceType.Desktop)
        {
            if (grabbing)
            {
                grabbingSlotUI.transform.position = pos;
                if (Input.GetMouseButtonDown(0))
                {
                    if (SearchSlot(out InventorySlotUI slot))
                    {
                        if (grabbingSlot.item is IApplied applied && slot.origin.item != null && applied.IsAppliableTo(slot.origin.item))
                        {
                            applied.ApplyTo(slot.origin.item);
                            return;
                        }
                        if (grabbingSlot.item.IsStackable(slot.origin.item)) grabbingSlot.count = slot.origin.Insert(grabbingSlot.item, grabbingSlot.count);
                        else if (grabbingSlot.Swap(slot.origin)) { }
                    }
                    else
                    {
                        DropItem(grabbingSlot.item, grabbingSlot.count);
                        grabbingSlot.count = 0;
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (SearchSlot(out InventorySlotUI slot))
                    {
                        if (grabbingSlot.item is IApplied applied && slot.origin.item != null && applied.IsAppliableTo(slot.origin.item))
                        {
                            applied.ApplyTo(slot.origin.item);
                            return;
                        }
                        grabbingSlot.count -= 1 - slot.origin.Insert(grabbingSlot.item, 1);
                    }
                    else
                    {
                        DropItem(grabbingSlot.item, 1);
                        grabbingSlot.count -= 1;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (SearchSlot(out InventorySlotUI slot))
                    {
                        if (slot.origin.item != null && slot.origin.Swap(grabbingSlot))
                        {
                            grabbingSlotUI.transform.position = Input.mousePosition;
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (SearchSlot(out InventorySlotUI slot))
                    {
                        if (slot.origin.item != null)
                        {
                            int count = slot.origin.count / 2 + slot.origin.count % 2;
                            slot.origin.count -= count - grabbingSlot.Insert(slot.origin.item, count);
                            grabbingSlotUI.transform.position = Input.mousePosition;
                        }
                    }
                }
            }
        }
        if (DeviceManager.deviceType == DeviceType.Handheld)
        {
            if (grabbing)
            {
                grabbingSlotUI.transform.position = pos;
                if (!UIManager.Instance.isTabOpen)
                {
                    if (prevSlot != null) grabbingSlot.count = prevSlot.Insert(grabbingSlot.item, grabbingSlot.count);
                    if (grabbingSlot.count > 0) player.inventory.InsertAndDropRest(grabbingSlot.item, grabbingSlot.count);
                    grabbingSlot.count = 0;
                    return;
                }
                if (!Input.GetMouseButton(0))
                {
                    if (SearchSlot(out InventorySlotUI slot))
                    {
                        if (grabbingSlot.item is IApplied applied && slot.origin.item != null && applied.IsAppliableTo(slot.origin.item)) applied.ApplyTo(slot.origin.item);
                        else grabbingSlot.count = slot.origin.Insert(grabbingSlot.item, grabbingSlot.count);
                        if (grabbingSlot.count > 0)
                        {
                            if (prevSlot != null) grabbingSlot.count = prevSlot.Insert(grabbingSlot.item, grabbingSlot.count);
                            if (grabbingSlot.count > 0) player.inventory.InsertAndDropRest(grabbingSlot.item, grabbingSlot.count);
                            grabbingSlot.count = 0;
                        }
                    }
                    else
                    {
                        DropItem(grabbingSlot.item, grabbingSlot.count);
                        grabbingSlot.count = 0;
                    }
                }
            }
            else
            {
                if (!UIManager.Instance.isTabOpen) return;
                if (Input.GetMouseButtonDown(0))
                {
                    if(SearchSlot(out InventorySlotUI slot))
                    {
                        if (slot.origin.item != null)
                        {
                            slot.origin.Swap(grabbingSlot);
                            prevSlot = slot.origin;
                            grabbingSlotUI.transform.position = Input.mousePosition;
                        }
                    }
                }
            }
        }
    }
    void DropItem(Item item, int count)
    {
        player.inventory.DropItem(item, count);
    }
}