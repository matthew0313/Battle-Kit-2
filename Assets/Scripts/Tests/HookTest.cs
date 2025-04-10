using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HookTest : MonoBehaviour
{
    [SerializeField] IRef<IHookProvider<float>> hookProvider;
    public IHookProvider<float> hook => hookProvider.I;
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
        Debug.Log($"Hook updated: {hook.value}");
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(HookTest))]
public class HookTest_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add 10 to hook")) (target as HookTest).hook.SetValue((target as HookTest).hook.value + 10);
    }
}
#endif