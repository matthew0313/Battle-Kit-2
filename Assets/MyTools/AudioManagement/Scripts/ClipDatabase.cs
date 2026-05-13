using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Sound Database", menuName = "Scriptables/Sound Database")]
public class SoundDatabase : ScriptableObject
{
    [SerializeField] List<SoundDatabaseElement> items;
    private void OnEnable()
    {
#if UNITY_EDITOR
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/SoundDatabaseIcon");
        EditorGUIUtility.SetIconForObject(this, icon);
#endif
    }
    public int size => items.Count;
    public IEnumerable<Sound> GetSounds()
    {
        foreach(var i in items)
        {
            if (i.type == SoundDatabaseElementType.Sound)
            {
                yield return i.Sound;
            }
            else if(i.type == SoundDatabaseElementType.SoundDatabase)
            {
                foreach (var k in i.SoundDatabase.GetSounds()) yield return k;
            }
        }
    }
    public int GetIndex(Sound sound)
    {
        int index = 0;
        foreach(var i in GetSounds())
        {
            if (i == sound) return index;
            index++;
        }
        throw new Exception("Unregistered ItemData");
    }
    public Sound GetSound(int index)
    {
        foreach(var i in GetSounds())
        {
            if (index-- == 0) return i;
        }
        throw new Exception("Index out of range");
    }
}
[System.Serializable]
public struct SoundDatabaseElement
{
    public SoundDatabaseElementType type;
    public Sound Sound;
    public SoundDatabase SoundDatabase;
}
[System.Serializable]
public enum SoundDatabaseElementType
{
    Sound = 0,
    SoundDatabase = 1
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SoundDatabaseElement))]
public class SoundDatabaseElement_Drawer : PropertyDrawer
{
    const int typeWidth = 110;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = typeWidth }, property.FindPropertyRelative("type"), GUIContent.none);

        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3}, property.FindPropertyRelative("sound"), GUIContent.none);
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3 }, property.FindPropertyRelative("soundDatabase"), GUIContent.none);
        }
    }
}
#endif