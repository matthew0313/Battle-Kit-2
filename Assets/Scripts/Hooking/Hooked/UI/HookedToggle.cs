using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedToggle : MonoBehaviour
{
    [SerializeField] IRef<IHookProvider<bool>> hook;

    [Header("Display")]
    [SerializeField] Toggle toggle;
    private void OnEnable()
    {
        hook.I.AssignCallback(OnValueChange);
        toggle.onValueChanged.AddListener(hook.I.SetValue);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.I.RemoveCallback(OnValueChange);
        toggle.onValueChanged.RemoveListener(hook.I.SetValue);
    }
    void OnValueChange()
    {
        toggle.isOn = hook.I.value;
    }
}