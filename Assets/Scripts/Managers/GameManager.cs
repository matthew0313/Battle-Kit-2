using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

    [SerializeField] Player playerPrefab;
    [SerializeField] Transform playerSpawnPoint;
    readonly List<Player> players = new();

    private void Start()
    {
        Player player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        player.NetworkObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, true);
    }
    public event Action onPlayerListChange;
    public void AddPlayer(Player player)
    {
        players.Add(player);
        onPlayerListChange?.Invoke();
    }
    public void RemovePlayer(Player player)
    {
        players.Remove(player);
        onPlayerListChange?.Invoke();
    }
}
