using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class HookedInputField : MonoBehaviour
{
    [SerializeField] HookedInputFieldSettings settings;

    [Header("Display")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button submitButton;
    private void OnEnable()
    {
        switch (settings.mode)
        {
            case HookedInputFieldMode.String:
                settings.stringHookProvider.I.AssignCallback(OnValueChange); break;
            case HookedInputFieldMode.Integer:
                settings.integerHookProvider.I.AssignCallback(OnValueChange); break;
            case HookedInputFieldMode.Float:
                settings.floatHookProvider.I.AssignCallback(OnValueChange); break;
        }
        inputField.onValueChanged.AddListener(OnTextChange);
        if (!settings.enterRequired) inputField.onValueChanged.AddListener(AutoSubmit);
        if(submitButton != null) submitButton.onClick.AddListener(Submit);
        OnValueChange();
    }
    private void OnDisable()
    {
        switch (settings.mode)
        {
            case HookedInputFieldMode.String:
                settings.stringHookProvider.I.RemoveCallback(OnValueChange); break;
            case HookedInputFieldMode.Integer:
                settings.integerHookProvider.I.RemoveCallback(OnValueChange); break;
            case HookedInputFieldMode.Float:
                settings.floatHookProvider.I.RemoveCallback(OnValueChange); break;
        }
        inputField.onValueChanged.RemoveListener(OnTextChange);
        if (!settings.enterRequired) inputField.onValueChanged.RemoveListener(AutoSubmit);
        if(submitButton != null) submitButton.onClick.RemoveListener(Submit);
    }
    void OnTextChange(string text)
    {
        if (submitButton == null) return;
        switch (settings.mode)
        {
            case HookedInputFieldMode.String:
                submitButton.interactable = true; break;
            case HookedInputFieldMode.Integer:
                submitButton.interactable = int.TryParse(text, out _); break;
            case HookedInputFieldMode.Float:
                submitButton.interactable = float.TryParse(text, out _); break;
        }
    }
    void AutoSubmit(string text) => Submit();
    public void Submit()
    {
        switch (settings.mode)
        {
            case HookedInputFieldMode.String:
                settings.stringHookProvider.I.SetValue(inputField.text); break;
            case HookedInputFieldMode.Integer:
                if (int.TryParse(inputField.text, out int integer)) settings.integerHookProvider.I.SetValue(integer); break;
            case HookedInputFieldMode.Float:
                if (float.TryParse(inputField.text, out float floatVal)) settings.floatHookProvider.I.SetValue(floatVal); break;
        }
    }
    void OnValueChange()
    {
        if (inputField.isFocused) return;
        switch (settings.mode)
        {
            case HookedInputFieldMode.String:
                inputField.text = settings.stringHookProvider.I.value; break;
            case HookedInputFieldMode.Integer:
                inputField.text = settings.integerHookProvider.I.value.ToString(); break;
            case HookedInputFieldMode.Float:
                inputField.text = settings.floatHookProvider.I.value.ToString(); break;
        }
    }
}
[System.Serializable]
public struct HookedInputFieldSettings
{
    public HookedInputFieldMode mode;

    //string
    public IRef<IHookProvider<string>> stringHookProvider;

    //integer
    public IRef<IHookProvider<int>> integerHookProvider;

    //float
    public IRef<IHookProvider<float>> floatHookProvider;

    public bool enterRequired;
}
[System.Serializable]
public enum HookedInputFieldMode
{
    String = 0,
    Integer = 1,
    Float = 2
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HookedInputFieldSettings))]
public class HookedInputFieldSettings_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("mode")) + 2.0f;
        position.y += tmp;

        if (property.FindPropertyRelative("mode").enumValueIndex == 0)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("stringHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;
        }
        else if (property.FindPropertyRelative("mode").enumValueIndex == 1)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("integerHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;
        }
        else if (property.FindPropertyRelative("mode").enumValueIndex == 2)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("floatHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;
        }
        tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("enterRequired")) + 2.0f;
        position.y += tmp;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0.0f;
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("mode")) + 2.0f;
        if (property.FindPropertyRelative("mode").enumValueIndex == 0)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("stringHookProvider").FindPropertyRelative("target"), true) + 2.0f;
        }
        else if (property.FindPropertyRelative("mode").enumValueIndex == 1)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("integerHookProvider").FindPropertyRelative("target"), true) + 2.0f;
        }
        else if (property.FindPropertyRelative("mode").enumValueIndex == 2)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("floatHookProvider").FindPropertyRelative("target"), true) + 2.0f;
        }
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("enterRequired")) + 2.0f;
        return height;
    }
}
#endif