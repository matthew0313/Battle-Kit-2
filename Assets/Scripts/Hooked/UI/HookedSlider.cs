using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedSlider : MonoBehaviour
{
    [SerializeField] FloatHook hook;

    [Header("Display")]
    [SerializeField] Slider slider;
    private void OnEnable()
    {
        hook.AssignCallback(OnValueChange);
        slider.onValueChanged.AddListener(hook.SetValue);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.RemoveCallback(OnValueChange);
        slider.onValueChanged.RemoveListener(hook.SetValue);
    }
    void OnValueChange()
    {
        slider.value = hook.value;
    }
}