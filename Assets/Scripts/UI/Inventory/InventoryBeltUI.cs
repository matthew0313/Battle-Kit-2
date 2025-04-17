using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBeltUI : MonoBehaviour, ILocalPlayerSpawnCallbackReceiver
{
    [SerializeField] InventorySlotUI[] slots = new InventorySlotUI[Player_Inventory.beltLength];
    [SerializeField] Transform equippedSlotIndicator;
    Player player;
    public void OnLocalPlayerSpawn(Player player)
    {
        this.player = player;
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].Set(player.inventory.slots[i]);
        }
    }
    private void Update()
    {
        equippedSlotIndicator.position = slots[player != null ? player.inventory.equippedSlotIndex : 0].transform.position;
    }
}