using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    public ItemManager() => Instance = this;

    [Header("Items")]
    [SerializeField] ItemDatabase m_itemDatabase;
    public ItemDatabase itemDatabase => m_itemDatabase;
    List<Item> items = new(), addQueue = new(), removeQueue = new();
    public void AddItem(Item item) => addQueue.Add(item);
    public void RemoveItem(Item item) => removeQueue.Remove(item);
    private void Update()
    {
        foreach (var i in addQueue) items.Add(i); addQueue.Clear();
        foreach (var i in removeQueue) items.Remove(i); removeQueue.Clear();
        foreach (var i in items) i.Update();
    }
}