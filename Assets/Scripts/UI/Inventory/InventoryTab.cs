using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTab : Tab, ILocalPlayerSpawnCallbackReceiver
{
    const int columns = 6;

    [SerializeField] CanvasGroup group;
    [SerializeField] float tweenTime = 0.25f;
    [SerializeField] InventorySlotUI[] row1 = new InventorySlotUI[columns], row2 = new InventorySlotUI[columns], row3 = new InventorySlotUI[columns];
    readonly List<List<InventorySlotUI>> slotUIs = new();
    InventorySlotUI selected, highlighted;
    int selectedX = 0, selectedY = 0;
    public void OnLocalPlayerSpawn(Player player)
    {
        slotUIs.Add(row3.ToList()); slotUIs.Add(row1.ToList()); slotUIs.Add(row2.ToList());
        int index = 0;
        for (int i = 0; i < slotUIs.Count; i++)
        {
            for (int k = 0; k < slotUIs[i].Count; k++)
            {
                if (index >= Player_Inventory.slotCount) return;
                slotUIs[i][k].Set(player.inventory.slots[index++]);
                Debug.Log($"{i}, {k}");
            }
        }
    }
    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        if (DeviceManager.deviceType == DeviceType.Console) Select(0, 0);

        group.interactable = true;
        group.blocksRaycasts = true;
        group.DOKill();
        group.DOFade(1.0f, tweenTime);
    }
    public override void Close()
    {
        base.Close();
        if (selected != null) selected.SetSelect(false);
        selected = null;

        group.interactable = false;
        group.blocksRaycasts = false;
        group.DOKill();
        group.DOFade(0.0f, tweenTime).OnComplete(() => gameObject.SetActive(false));
    }
    void Select(int x, int y)
    {
        if (y >= slotUIs.Count) y = 0; if (y < 0) y = slotUIs.Count - 1;
        if (x >= slotUIs[y].Count) x = 0; if (x < 0) x = slotUIs[y].Count - 1;

        if (selected != null) selected.SetSelect(false);
        selected = slotUIs[y][x];
        selected.SetSelect(true);
        selectedX = x; selectedY = y;
    }
    void SetHighlighted(InventorySlotUI slotUI)
    {
        if (highlighted != null) highlighted.SetHighlight(false);
        highlighted = slotUI;
        if(highlighted != null) highlighted.SetHighlight(true);
    }
    private void Update()
    {
        if (!isOpen) return;
        if (DeviceManager.deviceType == DeviceType.Console)
        {
            if (InputManager.Instance.SelectMoveInput().x > 0) Select(selectedX + 1, selectedY);
            if (InputManager.Instance.SelectMoveInput().x < 0) Select(selectedX - 1, selectedY);
            if (InputManager.Instance.SelectMoveInput().y > 0) Select(selectedX, selectedY - 1);
            if (InputManager.Instance.SelectMoveInput().y < 0) Select(selectedX, selectedY + 1);
            if (InputManager.Instance.SelectInput())
            {
                if (highlighted == null) SetHighlighted(selected);
                else
                {
                    if (highlighted != selected)
                    {
                        highlighted.origin.Swap(selected.origin);
                    }
                    SetHighlighted(null);
                }
            }
        }
    }
}