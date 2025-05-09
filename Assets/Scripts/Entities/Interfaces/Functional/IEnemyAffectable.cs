using Unity.Netcode;
using UnityEngine;

public interface IEnemyAffectable
{
    public bool IsEnemy(Player other);
}