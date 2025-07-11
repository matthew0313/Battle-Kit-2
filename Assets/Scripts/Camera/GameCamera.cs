using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class GameCamera : MonoBehaviour, ILocalPlayerSpawnCallbackReceiver
{
    Player player;
    public void OnLocalPlayerSpawn(Player player)
    {
        this.player = player;
        GetComponent<CinemachineCamera>().Follow = player.transform;
    }
}