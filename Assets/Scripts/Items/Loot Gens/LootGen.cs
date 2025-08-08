using System.Collections.Generic;
using UnityEngine;
public abstract class LootGen : ScriptableObject
{
    public abstract IEnumerable<ItemIntPair> GetLoot();
}
[System.Serializable]
public struct ItemIntPair
{
    public Item item;
    public int count;
}