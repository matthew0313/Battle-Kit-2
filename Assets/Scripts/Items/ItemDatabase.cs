using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Item Database", menuName = "Scriptables/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] List<ItemDatabaseElement> items;
    private void OnEnable()
    {
#if UNITY_EDITOR
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/ItemDatabaseIcon");
        EditorGUIUtility.SetIconForObject(this, icon);
#endif
    }
    public int size => items.Count;
    public IEnumerable<ItemData> GetItemDatas()
    {
        foreach(var i in items)
        {
            if (i.type == ItemDatabaseElementType.Item)
            {
                yield return i.item;
            }
            else if(i.type == ItemDatabaseElementType.ItemDatabase)
            {
                foreach (var k in i.itemDatabase.GetItemDatas()) yield return k;
            }
        }
    }
    public int GetIndex(ItemData item)
    {
        int index = 0;
        foreach(var i in GetItemDatas())
        {
            if (i == item) return index;
            index++;
        }
        throw new Exception("Unregistered ItemData");
    }
    public ItemData GetItemData(int index)
    {
        foreach(var i in GetItemDatas())
        {
            if (index-- == 0) return i;
        }
        throw new Exception("Index out of range");
    }
}
[System.Serializable]
public struct ItemDatabaseElement
{
    public ItemDatabaseElementType type;
    public ItemData item;
    public ItemDatabase itemDatabase;
}
[System.Serializable]
public enum ItemDatabaseElementType
{
    Item = 0,
    ItemDatabase = 1
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ItemDatabaseElement))]
public class ItemDatabaseElement_Drawer : PropertyDrawer
{
    const int typeWidth = 110;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = typeWidth }, property.FindPropertyRelative("type"), GUIContent.none);

        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3}, property.FindPropertyRelative("item"), GUIContent.none);
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3 }, property.FindPropertyRelative("itemDatabase"), GUIContent.none);
        }
    }
}
#endif