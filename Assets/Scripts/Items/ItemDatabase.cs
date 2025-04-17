using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "Scriptables/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] List<ItemData> items;
    public int GetIndex(ItemData item) => items.IndexOf(item);
    public ItemData GetItemData(int index) => items[index];
}