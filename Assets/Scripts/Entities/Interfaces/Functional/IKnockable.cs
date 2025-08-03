using Unity.Netcode;
using UnityEngine;

public interface IKnockable : IEntityAffectable
{
    public void GetKnockback(KnockbackInfo knockback);
}
[System.Serializable]
public struct KnockbackInfo : INetworkSerializable
{
    public Vector2 value;

    public KnockbackSource source;
    //source is player
    public ulong clientID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref value);
        serializer.SerializeValue(ref source);
        if (source == KnockbackSource.Player) serializer.SerializeValue(ref clientID);
    }
}
[System.Serializable]
public enum KnockbackSource
{
    Player = 0
}