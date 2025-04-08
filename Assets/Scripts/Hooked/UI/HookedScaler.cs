using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedScaler : MonoBehaviour
{
    [SerializeField] FloatHook XHook, YHook, ZHook;

    [Header("Display")]
    [SerializeField] Transform scaled;
    private void OnEnable()
    {
        if(XHook != null) XHook.AssignCallback(OnValueChange);
        if(YHook != null) YHook.AssignCallback(OnValueChange);
        if(ZHook != null) ZHook.AssignCallback(OnValueChange);
        OnValueChange();
    }
    private void OnDisable()
    {
        if(XHook != null) XHook.RemoveCallback(OnValueChange);
        if(YHook != null) YHook.RemoveCallback(OnValueChange);
        if(ZHook != null) ZHook.RemoveCallback(OnValueChange);
    }
    void OnValueChange()
    {
        scaled.localScale = new Vector3(
            XHook != null ? XHook.value : scaled.localScale.x,
            YHook != null ? YHook.value : scaled.localScale.y,
            ZHook != null ? ZHook.value : scaled.localScale.z);
    }
}