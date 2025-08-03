using System.Collections.Generic;
using UnityEngine;

public class TabOpener : MonoBehaviour
{
    [SerializeField] protected Tab currentTab;
    public virtual void OpenTab(Tab tab)
    {
        if (currentTab == tab || tab == null)
        {
            CloseTab();
            return;
        }

        if (currentTab != null) currentTab.Close();
        currentTab = tab;
        currentTab.Open();
    }
    public virtual void CloseTab()
    {
        if (currentTab != null) currentTab.Close();
        currentTab = null;
    }
}