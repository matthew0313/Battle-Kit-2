using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedEnabler : MonoBehaviour
{
    [SerializeField] IRef<IHookProvider<bool>> hook;
    [SerializeField] bool invert = false;

    [Header("Display")]
    [SerializeField] GameObject target;
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
        target.SetActive(invert ? !hook.I.value : hook.I.value);
    }
}