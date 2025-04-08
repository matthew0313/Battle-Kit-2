using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedVolumer : MonoBehaviour
{
    [SerializeField] FloatHook hook;
    [SerializeField] float multiplier = 1.0f;

    [Header("Output")]
    [SerializeField] AudioSource source;
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
        source.volume = hook.value * multiplier;
    }
}