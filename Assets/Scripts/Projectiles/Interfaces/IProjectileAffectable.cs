using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public interface IProjectileAffectable
{
    public Projectile self { get; }
}