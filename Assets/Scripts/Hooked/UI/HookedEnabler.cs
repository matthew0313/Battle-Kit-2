using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedEnabler : MonoBehaviour
{
    [SerializeField] BoolHook hook;
    [SerializeField] bool invert = false;

    [Header("Display")]
    [SerializeField] GameObject target;
    private void OnEnable()
    {
        hook.AssignCallback(OnValueChange);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.RemoveCallback(OnValueChange);
    }
    void OnValueChange()
    {
        target.SetActive(invert ? !hook.value : hook.value);
    }
}