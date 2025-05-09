using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct ItemInstanceData : INetworkSerializable
{
    public int itemIndex;
    public NetworkDictionary properties;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemIndex);
        serializer.SerializeValue(ref properties);
    }
    public Item ToItem()
    {
        Item tmp = ItemManager.Instance.itemDatabase.GetItemData(itemIndex).Create();
        tmp.LoadProperties(properties.RetrieveData());
        return tmp;
    }
}