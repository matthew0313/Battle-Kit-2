using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Default LootGen", menuName = "Scriptables/Loot Gens/Default")]
public class LootGen_Default : LootGen
{
    [SerializeField] List<LGDefault_Element> elements;

    public override IEnumerable<ItemIntPair> GetLoot()
    {
        int selected = UnityEngine.Random.Range(0, elements.Count);
        foreach(var i in elements[selected].items)
        {
            yield return new()
            {
                item = i.itemData.Create(),
                count = i.count
            };
        }
    }
}
[System.Serializable]
public struct ItemDataIntPair
{
    public ItemData itemData;
    public int count;
}
[System.Serializable]
public struct LGDefault_Element
{
    public string name;
    public List<ItemDataIntPair> items;
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ItemDataIntPair))]
public class ItemDataIntPair_Drawer : PropertyDrawer
{
    const int countWidth = 80;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = position.width - countWidth - 3 }, property.FindPropertyRelative("itemData"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position) { x = position.x + position.width - countWidth, width = countWidth }, property.FindPropertyRelative("count"), GUIContent.none);
    }
}
#endif