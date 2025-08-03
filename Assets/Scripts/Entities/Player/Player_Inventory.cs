using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
    [SerializeField] Transform dropAnchor;
    [SerializeField] float dropForce = 3.0f;
    [SerializeField] float dropPickupDelay = 1.0f;

    [Header("Slot Switch")]
    [SerializeField] float holdThreshold = 0.5f;
    [SerializeField] float slotSwitchRate = 0.1f;
    //Equipped Slot
    public int equippedSlotIndex { get; private set; }
    public InventorySlot equippedSlot { get; private set; }
    public Action onEquippedSlotChange;

    new public const int slotCount = 18;
    public const int beltLength = 6;

    //Equipped Item
    public Item equipped { get; private set; }
    public Action<Item> onEquippedItemChange;

    public bool canUseItem { get; private set; } = true;
    public ControlMachine<bool> canUseItemControl;

    public bool canSwitchSlot { get; private set; } = true;
    public ControlMachine<bool> canSwitchSlotControl;

    private void Awake()
    {
        origin = GetComponent<Player>();
        canUseItemControl = new(value => canUseItem = value, true);
        canSwitchSlotControl = new(value => canSwitchSlot = value, true);
    }
    public void Init()
    {
        if (IsOwner)
        {
            base.slotCount = slotCount;
            ChangeEquippedSlot(0);
        }
    }
    private void Update()
    {
        if (IsOwner)
        {
            UpdateBelt();


            if (equipped != null) equipped.OnWieldUpdate(origin);

            if (!canUseItem) return;
            if (equipped is ILMBUser lmbUser && InputManager.Instance.ItemUseInput(out bool down)) lmbUser.LMBUse(origin, down);
            if (equipped is ISkill1User skill1User && InputManager.Instance.Skill1UseInput()) skill1User.Skill1Use(origin);
            if (equipped is ISkill2User skill2User && InputManager.Instance.Skill2UseInput()) skill2User.Skill2Use(origin);
            if (equipped is IReloadable reloadable && InputManager.Instance.ReloadInput()) reloadable.Reload(origin);
        }

        foreach (var i in Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("DroppedItem")))
        {
            if (i.TryGetComponent(out DroppedItem item) && item.OwnerClientId == NetworkManager.Singleton.LocalClientId) item.Pickup(this);
        }
    }
    protected override IEnumerable<InventorySlot> GetSearchableSlots()
    {
        foreach (var i in base.GetSearchableSlots()) yield return i;
        yield return InventorySlotUIHandler.Instance.grabbingSlot;
    }
    int lastMoveDir = 0;
    float holdTimer = 0.0f, slotSwitchCounter = 0.0f;
    void UpdateBelt()
    {
        for(int i = 0; i < beltLength; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ChangeEquippedSlot(i); return;
            }
        }

        if (!UIManager.Instance.isTabOpen)
        {
            int moveDir = 0;
            if (InputManager.Instance.SelectMoveInput().x > 0) moveDir += 1;
            if (InputManager.Instance.SelectMoveInput().x < 0) moveDir -= 1;
            if(moveDir != 0)
            {
                if (moveDir != lastMoveDir)
                {
                    ChangeEquippedSlot(equippedSlotIndex + moveDir);
                    holdTimer = 0.0f; slotSwitchCounter = 0.0f;
                }
                else
                {
                    holdTimer += Time.deltaTime;
                    if(holdTimer >= holdThreshold)
                    {
                        slotSwitchCounter = Mathf.Max(slotSwitchCounter - Time.deltaTime, 0);
                        if(slotSwitchCounter <= 0.0f)
                        {
                            slotSwitchCounter = slotSwitchRate;
                            ChangeEquippedSlot(equippedSlotIndex + moveDir);
                        }
                    }
                }
            }
            lastMoveDir = moveDir;
        }
    }
    public void ChangeEquippedSlot(int index)
    {
        if (index < 0) index = beltLength - 1;
        if (index >= beltLength) index = 0;

        if (!canSwitchSlot || equippedSlot == slots[index]) return;

        if (equippedSlot != null) equippedSlot.onItemChange -= OnEquippedSlotUpdate;
        equippedSlot = slots[index];
        equippedSlotIndex = index;
        equippedSlot.onItemChange += OnEquippedSlotUpdate;
        onEquippedSlotChange?.Invoke();
        OnEquippedSlotUpdate();
        Debug.Log(index);
    }
    public void Disarm()
    {
        if (equippedSlot != null) equippedSlot.onItemChange -= OnEquippedSlotUpdate;
        equippedSlot = null;
        ChangeEquippedItem(null);
    }
    void OnEquippedSlotUpdate()
    {
        ChangeEquippedItem(equippedSlot.item);
    }
    void ChangeEquippedItem(Item item)
    {
        if (equipped != null) equipped.OnUnwield(origin);
        equipped = item;
        if (equipped != null) equipped.OnWield(origin);
        onEquippedItemChange?.Invoke(equipped);
    }
    public void DropItem(Item item, int count)
    {
        DroppedItem tmp = DroppedItem.Create();
        tmp.transform.position = dropAnchor.position;
        tmp.rb.linearVelocity = dropAnchor.right * dropForce;
        tmp.NetworkObject.Spawn();
        tmp.Set(item, count, dropPickupDelay);
    }
    public void InsertAndDropRest(Item item, int count)
    {
        int remaining = Insert(item, count);
        if (remaining > 0) DropItem(item.Copy(), remaining);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Player_Inventory))]
public class Player_Inventory_Editor : Editor
{
    ItemData givenItem;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        givenItem = EditorGUILayout.ObjectField(givenItem, typeof(ItemData), false) as ItemData;
        if (GUILayout.Button("Give Item")) (target as Player_Inventory).InsertRpc(givenItem.Create().ToInstanceData(), 1, null);
    }
}
#endif