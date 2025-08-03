using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : TabOpener
{
    public static UIManager Instance { get; private set; }
    public UIManager() => Instance = this;

    [SerializeField] InventoryTab inventoryTab;
    public bool isTabOpen => currentTab != null;
    private void Update()
    {
        if (Player.localPlayer == null) return;
        if (InputManager.Instance.CancelInput()) CloseTab();
        if (InputManager.Instance.InventoryOpenInput()) OpenTab(inventoryTab);
    }
}