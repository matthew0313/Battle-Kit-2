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
    public BoolHook boolHook;
    public Sprite trueSprite, falseSprite;

    //Int Hook
    public IntegerHook integerHook;
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
    float height = 0.0f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        height = 0.0f;

        float tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("mode")) + 2.0f;
        position.y += tmp; height += tmp;

        if (property.FindPropertyRelative("mode").enumValueIndex == 0)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("boolHook")) + 2.0f;
            position.y += tmp; height += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("trueSprite")) + 2.0f;
            position.y += tmp; height += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("falseSprite")) + 2.0f;
            position.y += tmp; height += tmp;
        }
        else if (property.FindPropertyRelative("mode").enumValueIndex == 1)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("integerHook")) + 2.0f;
            position.y += tmp; height += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("integerSprites")) + 2.0f;
            position.y += tmp; height += tmp;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return height;
    }
}
#endif