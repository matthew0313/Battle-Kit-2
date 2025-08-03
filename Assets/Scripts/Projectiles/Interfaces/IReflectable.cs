using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public interface IReflectable : IProjectileAffectable
{
    public void Reflect(Player reflector, Vector2 dir);
}