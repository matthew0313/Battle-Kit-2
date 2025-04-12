using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HookedScaler : MonoBehaviour
{
    [SerializeField] HookedScalerSettings settings;

    [Header("Display")]
    [SerializeField] Transform scaled;
    private void OnEnable()
    {
        if ((settings.controlledAxis & HookedScalerControlAxis.X) != 0) settings.XAxisHook.AssignCallback(OnValueChange);
        if ((settings.controlledAxis & HookedScalerControlAxis.Y) != 0) settings.YAxisHook.AssignCallback(OnValueChange);
        if ((settings.controlledAxis & HookedScalerControlAxis.Z) != 0) settings.ZAxisHook.AssignCallback(OnValueChange);
        OnValueChange();
    }
    private void OnDisable()
    {
        if ((settings.controlledAxis & HookedScalerControlAxis.X) != 0) settings.XAxisHook.RemoveCallback(OnValueChange);
        if ((settings.controlledAxis & HookedScalerControlAxis.Y) != 0) settings.YAxisHook.RemoveCallback(OnValueChange);
        if ((settings.controlledAxis & HookedScalerControlAxis.Z) != 0) settings.ZAxisHook.RemoveCallback(OnValueChange);
    }
    void OnValueChange()
    {
        scaled.localScale = new Vector3(
            (settings.controlledAxis & HookedScalerControlAxis.X) != 0 ? settings.XAxisHook.value : scaled.localScale.x,
            (settings.controlledAxis & HookedScalerControlAxis.Y) != 0 ? settings.YAxisHook.value : scaled.localScale.y,
            (settings.controlledAxis & HookedScalerControlAxis.Z) != 0 ? settings.ZAxisHook.value : scaled.localScale.z);
    }
}
[System.Serializable]
public struct HookedScalerSettings
{
    public HookedScalerControlAxis controlledAxis;
    [SerializeField] IRef<IHookProvider<float>> XAxisHookProvider, YAxisHookProvider, ZAxisHookProvider;
    public IHookProvider<float> XAxisHook => XAxisHookProvider.I;
    public IHookProvider<float> YAxisHook => YAxisHookProvider.I;
    public IHookProvider<float> ZAxisHook => ZAxisHookProvider.I;
}
[System.Serializable][Flags]
public enum HookedScalerControlAxis
{
    X = 1<<0,
    Y = 1<<1,
    Z = 1<<2
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HookedScalerSettings))]
public class HookedScalerSettings_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("controlledAxis")) + 2.0f;
        position.y += tmp;

        if (((HookedScalerControlAxis)property.FindPropertyRelative("controlledAxis").enumValueFlag & HookedScalerControlAxis.X) != 0)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("XAxisHookProvider").FindPropertyRelative("target"), new GUIContent() { text="X Hook"}) + 2.0f;
            position.y += tmp;
        }
        if (((HookedScalerControlAxis)property.FindPropertyRelative("controlledAxis").enumValueFlag & HookedScalerControlAxis.Y) != 0)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("YAxisHookProvider").FindPropertyRelative("target"), new GUIContent() { text = "Y Hook" }) + 2.0f;
            position.y += tmp;
        }
        if(((HookedScalerControlAxis)property.FindPropertyRelative("controlledAxis").enumValueFlag & HookedScalerControlAxis.Z) != 0)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("ZAxisHookProvider").FindPropertyRelative("target"), new GUIContent() { text = "Z Hook" }) + 2.0f;
            position.y += tmp;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0.0f;
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("controlledAxis")) + 2.0f;
        if (((HookedScalerControlAxis)property.FindPropertyRelative("controlledAxis").enumValueFlag & HookedScalerControlAxis.X) != 0)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("XAxisHookProvider")) + 2.0f;
        }
        if (((HookedScalerControlAxis)property.FindPropertyRelative("controlledAxis").enumValueFlag & HookedScalerControlAxis.Y) != 0)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("YAxisHookProvider")) + 2.0f;
        }
        if (((HookedScalerControlAxis)property.FindPropertyRelative("controlledAxis").enumValueFlag & HookedScalerControlAxis.Z) != 0)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("ZAxisHookProvider")) + 2.0f;
        }
        return height;
    }
}
#endif