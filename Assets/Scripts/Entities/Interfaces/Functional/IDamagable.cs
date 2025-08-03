using Unity.Netcode;
using UnityEngine;

public interface IDamagable : IEntityAffectable
{
    public void GetDamage(DamageInfo damage);
}
[System.Serializable]
public struct DamageInfo : INetworkSerializable
{
    public float amount;

    public DamageSource source;
    //source is player
    public ulong clientID;

    public bool hasColor;
    public Color color;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref amount);

        serializer.SerializeValue(ref source);
        if (source == DamageSource.Player) serializer.SerializeValue(ref clientID);

        serializer.SerializeValue(ref hasColor);
        if(hasColor) serializer.SerializeValue(ref color);
    }
}
/*public struct DamageReceivedInfo
{
    public bool ignored;

    public DamageInfo damage;
    public float receivedAmount;
    public bool fatal;
}*/

[System.Serializable]
public enum DamageSource
{
    Player = 0,
    Other = 1
}