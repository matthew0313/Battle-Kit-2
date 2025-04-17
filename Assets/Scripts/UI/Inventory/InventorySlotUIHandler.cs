using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUIHandler : MonoBehaviour, ILocalPlayerSpawnCallbackReceiver
{
    public static InventorySlotUIHandler Instance { get; private set; }
    public InventorySlotUIHandler() => Instance = this;

    Player player;
    InventorySlot grabbingSlot;

    [SerializeField] InventorySlotUI grabbingSlotUI;

    [Header("Describing")]
    [SerializeField] GameObject describer;

    public Item grabbing => grabbingSlot.item;
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
        
    }
    void DescUpdate()
    {
        if(grabbingSlot.item == null)
        {

        }
        else
        {
            describing = null;
            describer.SetActive(false);
        }
    }
    Item describing = null;
    void Describe(Item item)
    {
        describing = item;
    }
}