using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct ItemInstanceData : INetworkSerializable
{
    public int itemIndex;
    public NetworkDictionary data;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemIndex);
        serializer.SerializeValue(ref data);
    }
}