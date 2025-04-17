using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using MEC;
using Unity.VisualScripting;

[RequireComponent(typeof(IntegerReceiver), typeof(OwnerOnlyRigidbody2D))]
public class DroppedItem : NetworkBehaviour
{
    IntegerReceiver receiver;
    OwnerOnlyRigidbody2D ownerOnlyRb;

    NetworkVariable<ItemInstanceData> itemInstanceData = new NetworkVariable<ItemInstanceData>(new ItemInstanceData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<int> count = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    bool pending = false;

    [SerializeField] SpriteRenderer rend;
    [SerializeField] float pendingTimeout = 3.0f;
    [SerializeField] float pickupAvailableMagnitude = 0.5f;
    static DroppedItem prefab;
    public Rigidbody2D rb => ownerOnlyRb.rb;
    public static DroppedItem Create()
    {
        if (prefab == null) prefab = Resources.Load<DroppedItem>("DroppedItem");
        return Instantiate(prefab);
    }
    private void Awake()
    {
        receiver = GetComponent<IntegerReceiver>();
        ownerOnlyRb = GetComponent<OwnerOnlyRigidbody2D>();
        receiver.onResponseReceive += OnResponseReceive;
        itemInstanceData.OnValueChanged += OnItemSet;
        count.OnValueChanged += OnCountChange;
    }
    void OnItemSet(ItemInstanceData prev, ItemInstanceData data)
    {
        rend.sprite = ItemManager.Instance.itemDatabase.GetItemData(itemInstanceData.Value.itemIndex).itemIcon;
    }
    void OnCountChange(int prev, int data)
    {

    }
    public void Set(Item item, int count)
    {
        if (!IsOwner) return;
        itemInstanceData.Value = new ItemInstanceData()
        {
            itemIndex = ItemManager.Instance.itemDatabase.GetIndex(item.data),
            data = new NetworkDictionary().AssignData(item.Save())
        };
        this.count.Value = count;
    }
    CoroutineHandle pendingWaiting;
    public void Pickup(Inventory inventory)
    {
        if (!IsOwner || pending || rb.linearVelocity.magnitude > pickupAvailableMagnitude) return;

        Timing.KillCoroutines(pendingWaiting);
        pending = true;
        pendingWaiting = Timing.RunCoroutine(PendingWait());
        
        inventory.InsertRpc(itemInstanceData.Value, count.Value, new(receiver));
    }
    void OnResponseReceive(int remaining)
    {
        Timing.KillCoroutines(pendingWaiting);
        pending = false;
        if (remaining <= 0) NetworkObject.Despawn();
        else count.Value = remaining;
    }
    IEnumerator<float> PendingWait()
    {
        yield return Timing.WaitForSeconds(pendingTimeout);
        pending = false;
    }
}