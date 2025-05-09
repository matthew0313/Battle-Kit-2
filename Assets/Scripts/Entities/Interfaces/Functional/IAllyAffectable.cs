using Unity.Netcode;
using UnityEngine;

public interface IAllyAffectable
{
    public bool IsAlly(Player other);
}