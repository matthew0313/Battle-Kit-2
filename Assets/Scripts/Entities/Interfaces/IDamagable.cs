using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public interface IDamagable
{
    public Alliance alliance { get; }
    public void GetDamage(DamageInfo damage);
}
[System.Serializable]
public struct DamageInfo
{
    public float amount;

    public DamageSource damageSource;
    //source is player
    public ulong damageGiverClientID;
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