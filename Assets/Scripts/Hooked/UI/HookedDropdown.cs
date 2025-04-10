using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedDropdown : MonoBehaviour
{
    [SerializeField] IRef<IHookProvider<int>> hook;
    
    [Header("Display")]
    [SerializeField] TMP_Dropdown dropdown;
    private void OnEnable()
    {
        hook.I.AssignCallback(OnValueChange);
        dropdown.onValueChanged.AddListener(hook.I.SetValue);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.I.RemoveCallback(OnValueChange);
        dropdown.onValueChanged.RemoveListener(hook.I.SetValue);
    }
    void OnValueChange()
    {
        dropdown.value = hook.I.value;
    }
}