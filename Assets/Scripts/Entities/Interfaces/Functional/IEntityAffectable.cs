using Unity.Netcode;
using UnityEngine;

public interface IEntityAffectable
{
    public Entity self { get; }
}