using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;



#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Hook_StringBuilder", menuName = "Scriptables/Hooks/Composites/String Builder", order = 0)]
public class Hook_StringBuilder : StringHook
{
    [SerializeField] HookStringBuilderElement[] elements;
    StringBuilder sb = new();
    public override string value
    {
        get
        {
            sb.Clear();
            foreach (var element in elements)
            {
                switch (element.type)
                {
                    case HookStringBuilderElementType.Text:
                        sb.Append(element.text);
                        break;
                    case HookStringBuilderElementType.LangText:
                        sb.Append(element.langText.text);
                        break;
                    case HookStringBuilderElementType.IntegerHook:
                        sb.Append((element.integerHook.value * element.integerMultiplier).ToString());
                        break;
                    case HookStringBuilderElementType.FloatHook:
                        sb.Append((Mathf.Round(element.floatHook.value * element.floatMultiplier * Mathf.Pow(10, element.floatRounding)) / Mathf.Pow(10, element.floatRounding)).ToString());
                        break;
                    case HookStringBuilderElementType.StringHook:
                        sb.Append(element.stringHook.value);
                        break;
                }
            }
            return sb.ToString();
        }
    }

    public override void AssignCallback(Action callback)
    {
        bool languageDependant = false;
        foreach(var i in elements)
        {
            if (i.type == HookStringBuilderElementType.LangText) languageDependant = true;
            else if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.AssignCallback(callback);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.AssignCallback(callback);
            }
        }
        if (languageDependant) Settings.onLanguageChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        bool languageDependant = false;
        foreach(var i in elements)
        {
            if (i.type == HookStringBuilderElementType.LangText) languageDependant = true;
            else if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.RemoveCallback(callback);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.RemoveCallback(callback);
            }
        }
        if (languageDependant) Settings.onLanguageChange -= callback;
    }

    public override void SetValue(string value)
    {
        Debug.LogWarning("Unable to set value of string builder hook");
    }
}
[System.Serializable]
public struct HookStringBuilderElement
{
    public HookStringBuilderElementType type;

    //Text
    public string text;

    //LangText
    public LangText langText;

    //IntegerHook
    [SerializeField] IRef<IHookProvider<int>> integerHookProvider;
    public IHookProvider<int> integerHook => integerHookProvider.I;
    public int integerMultiplier;

    //FloatHook
    [SerializeField] IRef<IHookProvider<float>> floatHookProvider;
    public IHookProvider<float> floatHook => floatHookProvider.I;
    public float floatMultiplier;
    public int floatRounding;

    //StringHook
    [SerializeField] IRef<IHookProvider<string>> stringHookProvider;
    public IHookProvider<string> stringHook => stringHookProvider.I;
}
[System.Serializable]
public enum HookStringBuilderElementType
{
    Text = 0,
    LangText = 1,
    IntegerHook = 2,
    FloatHook = 3,
    StringHook = 4
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HookStringBuilderElement))]
public class HookStringBuilderElement_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("type")) + 2.0f;
        position.y += tmp;

        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("text")) + 2.0f;
            position.y += tmp;
        }
        else if(property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("langText")) + 2.0f;
            position.y += tmp;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 2)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("integerHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("integerMultiplier")) + 2.0f;
            position.y += tmp;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 3)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("floatHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("floatMultiplier")) + 2.0f;
            position.y += tmp;

            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("floatRounding")) + 2.0f;
            position.y += tmp;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 4)
        {
            tmp = MyEditor.PropertyFieldAndHeight(position, property.FindPropertyRelative("stringHookProvider").FindPropertyRelative("target")) + 2.0f;
            position.y += tmp;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0.0f;
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("type"), true) + 2;
        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text"), true) + 2;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("langText"), true) + 2;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 2)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("integerHookProvider").FindPropertyRelative("target"), true) + 2;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("integerMultiplier")) + 2;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 3)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("floatHookProvider").FindPropertyRelative("target"), true) + 2;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("floatMultiplier")) + 2;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("floatRounding")) + 2;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 4)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("stringHookProvider").FindPropertyRelative("target")) + 2;
        }
        return height;
    }
}
#endif