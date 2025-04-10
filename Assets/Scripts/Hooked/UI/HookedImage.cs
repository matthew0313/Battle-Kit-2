using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HookedImage : MonoBehaviour
{
    [SerializeField] HookedImageSettings settings;

    [Header("Display")]
    [SerializeField] Image target;
    private void OnEnable()
    {
        switch (settings.mode)
        {
            case HookedImageMode.BoolHook:
                settings.boolHook.AssignCallback(OnValueChange);
                break;
            case HookedImageMode.IntegerHook:
                settings.integerHook.AssignCallback(OnValueChange);
                break;
        }
    }
    private void OnDisable()
    {
        switch (settings.mode)
        {
            case HookedImageMode.BoolHook:
                settings.boolHook.RemoveCallback(OnValueChange);
                break;
            case HookedImageMode.IntegerHook:
                settings.integerHook.RemoveCallback(OnValueChange);
                break;
        }
    }
    void OnValueChange()
    {
        switch (settings.mode)
        {
            case HookedImageMode.BoolHook:
                target.sprite = settings.boolHook.value ? settings.trueSprite : settings.falseSprite;
                break;
            case HookedImageMode.IntegerHook:
                if (settings.integerHook.value < 0 || settings.integerHook.value >= settings.integerSprites.Length) return;
                target.sprite = settings.integerSprites[settings.integerHook.value];
                break;
        }
    }
}
[System.Serializable]
public struct HookedImageSettings
{
    public HookedImageMode mode;

    //Bool Hook
    [SerializeField] IRef<IHookProvider<bool>> boolHookProvider;
    public IHookProvider<bool> boolHook => boolHookProvider.I;
    public Sprite trueSprite, falseSprite;

    //Int Hook
    [SerializeField] IRef<IHookProvider<int>> integerHookProvider;
    public IHookProvider<int> integerHook => integerHookProvider.I;
    public Sprite[] integerSprites;
}
[System.Serializable]
public enum HookedImageMode
{
    BoolHook,
    IntegerHook
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HookedImageSettings))]
public class HookedImageSettings_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("mode")) + 2.0f;
        position.y += tmp;

        if (property.FindPropertyRelative("mode").enumValueIndex == 0)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("boolHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("trueSprite")) + 2.0f;
            position.y += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("falseSprite")) + 2.0f;
            position.y += tmp;
        }
        else if (property.FindPropertyRelative("mode").enumValueIndex == 1)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("integerHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("integerSprites")) + 2.0f;
            position.y += tmp;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0.0f;
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("mode")) + 2.0f;
        if (property.FindPropertyRelative("mode").enumValueIndex == 0)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("boolHookProvider").FindPropertyRelative("target"), true) + 2.0f;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("trueSprite"), true) + 2.0f;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("falseSprite"), true) + 2.0f;
        }
        else if (property.FindPropertyRelative("mode").enumValueIndex == 1)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("integerHookProvider").FindPropertyRelative("target"), true) + 2.0f;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("integerSprites"), true) + 2.0f;
        }
        return height;
    }
}
#endif