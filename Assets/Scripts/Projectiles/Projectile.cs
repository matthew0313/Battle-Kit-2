using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public abstract class Projectile : NetworkBehaviour
{
    public Player owner => IsSpawned ? Player.GetPlayer(OwnerClientId) : null;
}