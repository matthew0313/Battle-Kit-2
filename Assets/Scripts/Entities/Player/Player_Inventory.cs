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

    public bool canPickup { get; private set; } = true;
    public ControlMachine<bool> canPickupControl;

    private void Awake()
    {
        origin = GetComponent<Player>();
        canUseItemControl = new(value => canUseItem = value, true);
        canSwitchSlotControl = new(value => canSwitchSlot = value, true);
        canPickupControl = new(value => canPickup = value, true);
        GameManager.Instance.onRoundEnd += OnRoundEnd;
    }

    private void OnRoundEnd()
    {
        Clear();
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

            if (canUseItem)
            {
                if (equipped is ILMBUser lmbUser && InputManager.Instance.ItemUseInput(out bool down)) lmbUser.LMBUse(origin, down);
                if (equipped is ISkill1User skill1User && InputManager.Instance.Skill1UseInput()) skill1User.Skill1Use(origin);
                if (equipped is ISkill2User skill2User && InputManager.Instance.Skill2UseInput()) skill2User.Skill2Use(origin);
                if (equipped is IReloadable reloadable && InputManager.Instance.ReloadInput()) reloadable.Reload(origin);
            }

            if(equippedSlot != null && equippedSlot.item != null && InputManager.Instance.DropInput())
            {
                DropItem(equippedSlot.item, 1);
                equippedSlot.count--;
            }

            if (canPickup)
            {
                foreach (var i in Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("DroppedItem")))
                {
                    if (i.TryGetComponent(out DroppedItem item)) item.PickupRpc(this);
                }
            }
        }
    }
    public override int Insert(Item item, int count)
    {
        foreach (var i in slots)
        {
            if (i.item != null && i.item.IsStackable(item) && item.IsStackable(i.item))
            {
                count = i.Insert(item, count);
                if (count <= 0) return 0;
            }
        }
        if(item is IUsable)
        {
            foreach (var i in slots)
            {
                count = i.Insert(item, count);
                if (count <= 0) return 0;
            }
        }
        else
        {
            for(int i = beltLength; i < slotCount; i++)
            {
                count = slots[i].Insert(item, count);
                if (count <= 0) return 0;
            }
            for(int i = 0; i < beltLength; i++)
            {
                count = slots[i].Insert(item, count);
                if (count <= 0) return 0;
            }
        }
        return count;
    }
    public override int TryInsert(Item item, int count)
    {
        foreach (var i in slots)
        {
            if (i.item != null && i.item.IsStackable(item) && item.IsStackable(i.item))
            {
                count = i.TryInsert(item, count);
                if (count <= 0) return 0;
            }
        }
        if (item is IUsable)
        {
            foreach (var i in slots)
            {
                count = i.TryInsert(item, count);
                if (count <= 0) return 0;
            }
        }
        else
        {
            for (int i = beltLength; i < slotCount; i++)
            {
                count = slots[i].TryInsert(item, count);
                if (count <= 0) return 0;
            }
            for (int i = 0; i < beltLength; i++)
            {
                count = slots[i].TryInsert(item, count);
                if (count <= 0) return 0;
            }
        }
        return count;
    }
    public override int Search(Func<Item, bool> predicate)
    {
        var slot = InventorySlotUIHandler.Instance.grabbingSlot;
        return base.Search(predicate) + (predicate.Invoke(slot.item) ? slot.count : 0);
    }
    public override IEnumerable<InventorySlot> SearchAll(Func<Item, bool> predicate)
    {
        var slot = InventorySlotUIHandler.Instance.grabbingSlot;
        foreach (var i in base.SearchAll(predicate)) yield return i;
        if (slot.item != null && predicate.Invoke(slot.item)) yield return slot;
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
    public void DropHalf()
    {
        foreach(var i in slots)
        {
            if(i.item != null)
            {
                if (i.count > 1)
                {
                    DroppedItem tmp = DroppedItem.Create();
                    tmp.transform.position = transform.position;
                    float angle = UnityEngine.Random.Range(0, 360.0f);
                    tmp.rb.linearVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dropForce;
                    tmp.NetworkObject.Spawn();
                    tmp.Set(i.item, i.count / 2, dropPickupDelay);
                    i.count -= i.count / 2;
                }
                else if(UnityEngine.Random.Range(0.0f, 100.0f) <= 50.0f)
                {
                    DroppedItem tmp = DroppedItem.Create();
                    tmp.transform.position = transform.position;
                    float angle = UnityEngine.Random.Range(0, 360.0f);
                    tmp.rb.linearVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dropForce;
                    tmp.NetworkObject.Spawn();
                    tmp.Set(i.item, 1, dropPickupDelay);
                    i.count = 0;
                }
            }
        }
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