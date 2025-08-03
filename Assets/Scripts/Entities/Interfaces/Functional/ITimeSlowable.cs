using Unity.Netcode;
using UnityEngine;

public interface ITimeSlowable : IEntityAffectable
{
    public void SlowTime(float scale);
    public void RevertTime(float scale);
}