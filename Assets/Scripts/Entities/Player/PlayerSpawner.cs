using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] NetworkObject playerPrefab;
    private void Awake()
    {
        NetworkManager.Singleton.OnConnectionEvent += OnConnection;
    }
    void OnConnection(NetworkManager manager, ConnectionEventData data)
    {
        if(data.EventType == ConnectionEvent.ClientConnected && data.ClientId == manager.LocalClientId)
        {
            var player = Instantiate(playerPrefab);
            player.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
            player.transform.position = transform.position;
        }
    }
}