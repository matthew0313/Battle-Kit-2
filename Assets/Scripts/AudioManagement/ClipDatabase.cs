using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Clip Database", menuName = "Scriptables/Clip Database")]
public class ClipDatabase : ScriptableObject
{
    [SerializeField] List<ClipDatabaseElement> items;
    private void OnEnable()
    {
#if UNITY_EDITOR
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/ClipDatabaseIcon");
        EditorGUIUtility.SetIconForObject(this, icon);
#endif
    }
    public int size => items.Count;
    public IEnumerable<AudioClip> GetClips()
    {
        foreach(var i in items)
        {
            if (i.type == ClipDatabaseElementType.Clip)
            {
                yield return i.clip;
            }
            else if(i.type == ClipDatabaseElementType.ClipDatabase)
            {
                foreach (var k in i.clipDatabase.GetClips()) yield return k;
            }
        }
    }
    public int GetIndex(AudioClip clip)
    {
        int index = 0;
        foreach(var i in GetClips())
        {
            if (i == clip) return index;
            index++;
        }
        throw new Exception("Unregistered ItemData");
    }
    public AudioClip GetClip(int index)
    {
        foreach(var i in GetClips())
        {
            if (index-- == 0) return i;
        }
        throw new Exception("Index out of range");
    }
}
[System.Serializable]
public struct ClipDatabaseElement
{
    public ClipDatabaseElementType type;
    public AudioClip clip;
    public ClipDatabase clipDatabase;
}
[System.Serializable]
public enum ClipDatabaseElementType
{
    Clip = 0,
    ClipDatabase = 1
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ClipDatabaseElement))]
public class ClipDatabaseElement_Drawer : PropertyDrawer
{
    const int typeWidth = 110;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = typeWidth }, property.FindPropertyRelative("type"), GUIContent.none);

        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3}, property.FindPropertyRelative("clip"), GUIContent.none);
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3 }, property.FindPropertyRelative("clipDatabase"), GUIContent.none);
        }
    }
}
#endif