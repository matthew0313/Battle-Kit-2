using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedVolumer : MonoBehaviour
{
    [SerializeField] IRef<IHookProvider<float>> hook;
    [SerializeField] float multiplier = 1.0f;

    [Header("Output")]
    [SerializeField] AudioSource source;
    private void OnEnable()
    {
        hook.I.AssignCallback(OnValueChange);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.I.RemoveCallback(OnValueChange);
    }
    void OnValueChange()
    {
        source.volume = hook.I.value * multiplier;
    }
}