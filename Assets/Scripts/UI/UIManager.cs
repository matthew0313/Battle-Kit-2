using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : TabOpener
{
    public static UIManager Instance { get; private set; }
    public UIManager() => Instance = this;
    public bool isTabOpen => currentTab != null;
}