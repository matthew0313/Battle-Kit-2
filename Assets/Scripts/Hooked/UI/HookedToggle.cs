using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedToggle : MonoBehaviour
{
    [SerializeField] BoolHook hook;

    [Header("Display")]
    [SerializeField] Toggle toggle;
    private void OnEnable()
    {
        hook.AssignCallback(OnValueChange);
        toggle.onValueChanged.AddListener(hook.SetValue);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.RemoveCallback(OnValueChange);
        toggle.onValueChanged.RemoveListener(hook.SetValue);
    }
    void OnValueChange()
    {
        toggle.isOn = hook.value;
    }
}