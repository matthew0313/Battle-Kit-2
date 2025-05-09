using System.Collections.Generic;
using Unity.Collections;
using System;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct AlimentInstanceData : INetworkSerializable
{
    public AlimentType type;
    public NetworkDictionary properties;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref properties);
    }
    public Aliment ToAliment()
    {
        Aliment tmp = Aliment.GetAliment(type);
        tmp.LoadProperties(properties.RetrieveData());
        return tmp;
    }
}