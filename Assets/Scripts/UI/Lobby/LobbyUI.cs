using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Multiplayer;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Selectable[] defaultSelectables;

    Tab currentTab;
    public void OpenTab(Tab tab)
    {
        if (currentTab == tab || tab == null)
        {
            CloseTab();
            return;
        }

        if (currentTab != null) currentTab.Close();
        currentTab = tab;
        currentTab.Open();

        if (DeviceManager.deviceType == DeviceType.Console)
        {
            foreach (var i in defaultSelectables) i.interactable = false;
        }
    }
    public void CloseTab()
    {
        if (currentTab != null) currentTab.Close();
        currentTab = null;

        if (DeviceManager.deviceType == DeviceType.Console)
        {
            foreach (var i in defaultSelectables) i.interactable = true;
            defaultSelectables[0].Select();
        }
    }

    static bool introPlayed = false;
    readonly int introID = Animator.StringToHash("Intro");
    private void Awake()
    {
        if (!introPlayed)
        {
            anim.SetTrigger(introID);
            introPlayed = true;
        }
        if(DeviceManager.deviceType == DeviceType.Console) defaultSelectables[0].Select();
    }
    private void Update()
    {
        if (InputManager.Instance.CancelInput()) CloseTab();
    }
}
