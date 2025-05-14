using Unity.Netcode;
using UnityEngine;

public interface ITimeSlowable : IEnemyAffectable
{
    public void SlowTime(float scale);
    public void RevertTime(float scale);
}