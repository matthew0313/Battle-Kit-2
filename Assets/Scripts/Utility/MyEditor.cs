#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
public static class MyEditor
{
    public static float PropertyFieldAndHeight(Rect position, SerializedProperty property, GUIContent label = null)
    {
        float height = EditorGUI.GetPropertyHeight(property);
        EditorGUI.PropertyField(new Rect(position) { height = height }, property, label, true);
        return height;
    }
}
#endif