using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player_Inventory : Inventory
{
    Player origin;

    [Header("Pickup")]
    [SerializeField] float pickupRange;

    [Header("Weapons")]
    [SerializeField] Transform m_weaponAnchor;
    public Transform weaponAnchor => m_weaponAnchor;

    [Header("Drop")]
    [SerializeField] float dropForce = 3.0f;
    //Equipped Slot
    public int equippedSlotIndex { get; private set; }
    public InventorySlot equippedSlot { get; private set; }
    public Action onEquippedSlotChange;
    public const int beltLength = 6;

    //Equipped Item
    public Item equipped { get; private set; }
    public Action<Item> onEquippedItemChange;
    public void OnAwake()
    {
        origin = GetComponent<Player>();
    }
    public void OnStart()
    {
        if (IsOwner)
        {
            Init();
            ChangeEquippedSlot(0);
        }
    }
    public void OnUpdate()
    {
        if (IsOwner)
        {
            UpdateBelt();

            if (equipped != null) equipped.OnWieldUpdate(origin);
            if (equipped is ISkill1User skill1User && skill1User.canUseSkill1 && InputManager.Instance.Skill1UseInput()) skill1User.Skill1Use(origin);
            if (equipped is ISkill2User skill2User && skill2User.canUseSkill2 && InputManager.Instance.Skill2UseInput()) skill2User.Skill2Use(origin);
            if (equipped is IReloadable reloadable && reloadable.canReload && InputManager.Instance.ReloadInput()) reloadable.Reload(origin);
        }

        foreach (var i in Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("DroppedItem")))
        {
            if (i.TryGetComponent(out DroppedItem item) && item.OwnerClientId == this.OwnerClientId) item.Pickup(this);
        }
    }
    void UpdateBelt()
    {
        if (InputManager.Instance.BeltSelectionInput(out int index)) ChangeEquippedSlot(index);
    }
    void ChangeEquippedSlot(int index)
    {
        if (index < 0 || index >= beltLength) return;

        if (equippedSlot != null) equippedSlot.onItemChange -= OnEquippedSlotUpdate;
        equippedSlot = slots[index];
        equippedSlotIndex = index;
        equippedSlot.onItemChange += OnEquippedSlotUpdate;
        onEquippedSlotChange?.Invoke();
        OnEquippedSlotUpdate(null, equippedSlot.item);
    }
    void OnEquippedSlotUpdate(Item prevItem, Item newItem)
    {
        ChangeEquippedItem(newItem);
    }
    void ChangeEquippedItem(Item item)
    {
        if (equipped != null) equipped.OnUnwield(origin);
        equipped = item;
        if (equipped != null) equipped.OnWield(origin);
        onEquippedItemChange?.Invoke(equipped);
    }
    public void DropItem(Item item, int count, Vector2 dir)
    {
        DroppedItem tmp = DroppedItem.Create();
        tmp.transform.position = transform.position;
        tmp.rb.linearVelocity = dir * dropForce;
        tmp.NetworkObject.Spawn();
        tmp.Set(item, count);
    }
    [Header("Debug")]
    [SerializeField] ItemData debugGivenItem;
    public void DebugGiveItem()
    {
        InsertRpc(new ItemInstanceData()
        {
            itemIndex = ItemManager.Instance.itemDatabase.GetIndex(debugGivenItem),
            data = new()
        }, 1, new());
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Player_Inventory))]
public class Player_Inventory_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Give Item")) (target as Player_Inventory).DebugGiveItem();
    }
}
#endif