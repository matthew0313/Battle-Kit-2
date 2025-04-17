using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] NetworkObject playerPrefab;
    private void Awake()
    {
        NetworkManager.Singleton.OnConnectionEvent += OnConnection;
    }
    private void OnDestroy()
    {
        NetworkManager.Singleton.OnConnectionEvent -= OnConnection;
    }
    void OnConnection(NetworkManager manager, ConnectionEventData data)
    {
        if(data.EventType == ConnectionEvent.ClientConnected && data.ClientId == manager.LocalClientId)
        {
            Instantiate(playerPrefab).SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
    }
}