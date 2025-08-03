using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBeltSlotUI : InventorySlotUI, IPointerDownHandler
{
    Player player;
    int beltIndex;
    public void Set(InventorySlot slot, Player player, int beltIndex)
    {
        base.Set(slot);
        this.player = player;
        this.beltIndex = beltIndex;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (player == null) return;
        if(DeviceManager.deviceType == DeviceType.Handheld)
        {
            if (!UIManager.Instance.isTabOpen) player.inventory.ChangeEquippedSlot(beltIndex);
        }
    }
}