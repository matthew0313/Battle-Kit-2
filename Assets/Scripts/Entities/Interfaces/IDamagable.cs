using Unity.Netcode;
using UnityEngine;

public interface IDamagable
{
    public ulong ownerID { get; }
    public DamageReceivedInfo GetDamage(DamageInfo damage);
}
[System.Serializable]
public struct DamageInfo : INetworkSerializable
{
    public float amount;

    public DamageSource damageSource;
    //source is player
    public ulong damageGiverClientID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref damageSource);
        if (damageSource == DamageSource.Player) serializer.SerializeValue(ref damageGiverClientID);
    }
}
public struct DamageReceivedInfo
{
    public bool ignored;

    public DamageInfo damage;
    public float receivedAmount;
    public bool fatal;
}

[System.Serializable]
public enum DamageSource
{
    Player = 0
}

[System.Serializable]
public enum Alliance
{
    None = 0,
    TeamRed = 1,
    TeamBlue = 2,
    TeamGreen = 3,
    TeamYellow = 4
}