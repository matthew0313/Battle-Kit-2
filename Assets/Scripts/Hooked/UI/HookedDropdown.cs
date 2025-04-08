using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedDropdown : MonoBehaviour
{
    [SerializeField] IntegerHook hook;
    
    [Header("Display")]
    [SerializeField] TMP_Dropdown dropdown;
    private void OnEnable()
    {
        hook.AssignCallback(OnValueChange);
        dropdown.onValueChanged.AddListener(hook.SetValue);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.RemoveCallback(OnValueChange);
        dropdown.onValueChanged.RemoveListener(hook.SetValue);
    }
    void OnValueChange()
    {
        dropdown.value = hook.value;
    }
}