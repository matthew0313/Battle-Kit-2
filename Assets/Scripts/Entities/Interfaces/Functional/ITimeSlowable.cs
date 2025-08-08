using Unity.Netcode;
using UnityEngine;

public interface ITimeSlowable : IRelationInteraction
{
    public void SlowTime(float scale);
    public void RevertTime(float scale);
}