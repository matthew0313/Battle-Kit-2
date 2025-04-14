using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct ItemInstanceData : INetworkSerializable
{
    public int itemIndex;
    public List<StringKeyValuePair> data;
    int dataCount;
    public void AssignData(Dictionary<string, string> data)
    {
        if (this.data == null) this.data = new();
        this.data.Clear();
        foreach(var i in data)
        {
            this.data.Add(new StringKeyValuePair() { key = i.Key, value = i.Value });
        }
    }
    public Dictionary<string, string> RetrieveData()
    {
        if (this.data == null) return null;
        Dictionary<string, string> data = new();
        foreach(var i in this.data)
        {
            data.Add(i.key.ToString(), i.value.ToString());
        }
        return data;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemIndex);
        if (serializer.IsWriter)
        {
            dataCount = data != null ? data.Count : 0;
            serializer.SerializeValue(ref dataCount);
            for(int i = 0; i < dataCount; i++)
            {
                StringKeyValuePair pair = data[i];
                pair.NetworkSerialize(serializer);
            }
        }
        if (serializer.IsReader)
        {
            serializer.SerializeValue(ref dataCount);
            for(int i = 0; i < dataCount; i++)
            {
                StringKeyValuePair pair = new();
                pair.NetworkSerialize(serializer);
                data.Add(pair);
            }
        }
    }
}
[System.Serializable]
public struct StringKeyValuePair : INetworkSerializable
{
    public FixedString64Bytes key;
    public FixedString128Bytes value;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref key);
        serializer.SerializeValue(ref value);
    }
}