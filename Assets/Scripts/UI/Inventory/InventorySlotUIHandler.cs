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

    [Header("Describing")]
    [SerializeField] GameObject describer;
    [SerializeField] TMP_Text itemName, itemDesc;

    public InventorySlot grabbingSlot { get; private set; }
    bool grabbing => grabbingSlot.item != null;
    public void OnLocalPlayerSpawn(Player player)
    {
        this.player = player;
        grabbingSlot = new InventorySlot(player.inventory);
        grabbingSlotUI.Set(grabbingSlot);
    }
    private void Update()
    {
        if (player == null) return;
        GrabUpdate();
    }
    void GrabUpdate()
    {
        Vector2 pos = InputManager.Instance.MouseInput();
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (!UIManager.Instance.isTabOpen)
            {
                if (grabbing)
                {
                    DropItem(grabbingSlot.item, grabbingSlot.count);
                    grabbingSlot.count = 0;
                }
                return;
            }
        }
        if (!grabbing)
        {
            if (InputManager.Instance.ItemGrabInput())
            {
                List<RaycastResult> results = UIScanner.ScanUI(pos);
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        if (slot.origin.item != null && slot.origin.Swap(grabbingSlot))
                        {
                            grabbingSlotUI.transform.position = Input.mousePosition;
                            break;
                        }
                    }
                }
            }
            else if (InputManager.Instance.ItemHalfGrabInput())
            {
                List<RaycastResult> results = UIScanner.ScanUI(pos);
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        if (slot.origin.item != null && grabbingSlot.CanSetItem(slot.origin.item))
                        {
                            int count = slot.origin.count / 2 + slot.origin.count % 2;
                            grabbingSlot.SetItem(slot.origin.item.Copy());
                            grabbingSlot.count = count;
                            slot.origin.count -= count;
                            grabbingSlotUI.transform.position = Input.mousePosition;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            grabbingSlotUI.transform.position = pos;
            if (InputManager.Instance.ItemUngrabInput())
            {
                List<RaycastResult> results = UIScanner.ScanUI(pos);
                bool slotHit = false;
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        if (grabbingSlot.item is IApplied applied && slot.origin.item != null && applied.IsAppliableTo(slot.origin.item))
                        {
                            applied.ApplyTo(slot.origin.item);
                            return;
                        }
                        slotHit = true;
                        if (grabbingSlot.item.IsStackable(slot.origin.item))
                        {
                            grabbingSlot.count = slot.origin.Insert(grabbingSlot.item, grabbingSlot.count);
                            break;
                        }
                        else if (grabbingSlot.Swap(slot.origin))
                        {
                            break;
                        }
                    }
                }
                if (!slotHit && grabbingSlot.item.data.canDrop)
                {
                    DropItem(grabbingSlot.item.Copy(), grabbingSlot.count);
                    grabbingSlot.count = 0;
                }
            }
            else if (InputManager.Instance.ItemHalfUngrabInput())
            {
                List<RaycastResult> results = UIScanner.ScanUI(pos);
                bool slotHit = false;
                foreach (var i in results)
                {
                    if (i.gameObject.TryGetComponent(out InventorySlotUI slot))
                    {
                        slotHit = true;
                        grabbingSlot.count -= 1 - slot.origin.Insert(grabbingSlot.item.Copy(), 1);
                        break;
                    }
                }
                if (!slotHit && grabbingSlot.item.data.canDrop)
                {
                    DropItem(grabbingSlot.item.Copy(), 1);
                    grabbingSlot.count--;
                }
            }
        }
    }
    void DropItem(Item item, int count)
    {
        player.inventory.DropItem(item, count, ((Vector2)Camera.main.ScreenToWorldPoint(InputManager.Instance.MouseInput()) - (Vector2)player.transform.position).normalized);
    }
}