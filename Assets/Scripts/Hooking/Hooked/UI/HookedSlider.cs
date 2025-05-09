using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedSlider : MonoBehaviour
{
    [SerializeField] IRef<IHookProvider<float>> hook;

    [Header("Display")]
    [SerializeField] Slider slider;
    private void OnEnable()
    {
        hook.I.AssignCallback(OnValueChange);
        slider.onValueChanged.AddListener(hook.I.SetValue);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.I.RemoveCallback(OnValueChange);
        slider.onValueChanged.RemoveListener(hook.I.SetValue);
    }
    void OnValueChange()
    {
        slider.value = hook.I.value;
    }
}