using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScanner : MonoBehaviour
{
    static UIScanner Instance;
    static List<RaycastResult> hits = new();
    bool scanned = false;
    public static List<RaycastResult> ScanUI()
    {
        if(Instance == null)
        {
            Instance = new GameObject().AddComponent<UIScanner>();
            DontDestroyOnLoad(Instance);
        }
        if (!Instance.scanned)
        {
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, hits);
            Instance.scanned = true;
        }
        return hits;
    }
    private void Update()
    {
        scanned = false;
    }
}