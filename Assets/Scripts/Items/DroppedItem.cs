using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using MEC;
using Unity.VisualScripting;

[RequireComponent(typeof(IntegerReceiver))]
public class DroppedItem : NetworkBehaviour
{
    IntegerReceiver receiver;
    OwnerOnlyRigidbody2D ownerOnlyRb;

    NetworkVariable<ItemInstanceData> itemInstanceData = new NetworkVariable<ItemInstanceData>(new ItemInstanceData());
    NetworkVariable<int> count = new NetworkVariable<int>(0);
    bool pending = false;

    [SerializeField] SpriteRenderer rend1, rend2;
    [SerializeField] float pendingTimeout = 3.0f;
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
    private void OnEnable()
    {
        GameManager.Instance.onRoundEnd += OnRoundEnd;
    }
    private void OnDisable()
    {
        GameManager.Instance.onRoundEnd -= OnRoundEnd;
    }
    void OnRoundEnd() { if (IsOwner) NetworkObject.Despawn(); }
    private void Update()
    {
        if (!IsOwner) return;
        pickupDelay -= Time.deltaTime;
    }
    public Item item { get; private set; }
    void OnItemSet(ItemInstanceData prev, ItemInstanceData data)
    {
        item = ItemManager.Instance.itemDatabase.GetItemData(itemInstanceData.Value.itemIndex).Create();
        item.LoadProperties(itemInstanceData.Value.properties.RetrieveData());
        rend1.sprite = item.data.itemIcon; rend2.sprite = item.data.itemIcon;
    }
    void OnCountChange(int prev, int data)
    {
        rend2.gameObject.SetActive(data > 1);
    }
    float pickupDelay = 0.0f;
    public void Set(Item item, int count, float pickupDelay = 0.0f)
    {
        if (!IsOwner) return;
        itemInstanceData.Value = item.ToInstanceData();
        this.count.Value = count;
        this.pickupDelay = pickupDelay;
    }
    CoroutineHandle pendingWaiting;
    [Rpc(SendTo.Owner)]
    public void PickupRpc(NetworkBehaviourReference inventoryRef)
    {
        if (pending || pickupDelay > 0.0f) return;
        if(inventoryRef.TryGet(out Inventory inventory))
        {
            pending = true;
            pendingWaiting = Timing.RunCoroutine(PendingWait());

            inventory.InsertRpc(itemInstanceData.Value, count.Value, new(receiver));
        }
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