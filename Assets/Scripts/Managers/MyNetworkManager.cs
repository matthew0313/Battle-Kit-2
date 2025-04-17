using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
public class MyNetworkManager : MonoBehaviour
{
    public static MyNetworkManager Instance { get; private set; }
    public MyNetworkManager() => Instance = this;
    [SerializeField] GameObject playerPrefab;
    NetworkManager networkManager;
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
    }
    private void OnEnable()
    {
        //networkManager.OnClientStarted += ClientStarted;
        //networkManager.OnServerStarted += ServerStarted;
        SessionManager.Instance.onSessionChange += OnSessionChange;
        networkManager.OnConnectionEvent += OnConnection;
        //networkManager.OnSessionOwnerPromoted += OnSessionOwnerPromoted;
    }
    private void OnDisable()
    {
        //networkManager.OnClientStarted -= ClientStarted;
        //networkManager.OnServerStarted -= ServerStarted;
        SessionManager.Instance.onSessionChange -= OnSessionChange;
        networkManager.OnConnectionEvent -= OnConnection;
        //networkManager.OnSessionOwnerPromoted -= OnSessionOwnerPromoted;
    }
    ISession currentSession;
    void OnSessionChange(ISession session)
    {
        if (session != null)
        {
            //if (session.IsHost) networkManager.StartHost();
            //else networkManager.StartClient();
            //networkManager.StartClient();
            SceneManager.LoadScene("Game");
            Debug.Log(networkManager.NetworkConfig.NetworkTransport);
        }
        else SceneManager.LoadScene("Lobby");
    }
    void OnConnection(NetworkManager manager, ConnectionEventData data)
    {
        Debug.Log("Connection Event");
        /*if (data.EventType == ConnectionEvent.ClientConnected && manager.LocalClient.IsSessionOwner)
        {
            manager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
        if(data.EventType == ConnectionEvent.ClientConnected && data.ClientId == manager.LocalClientId)
        {
            Instantiate(playerPrefab).GetComponent<NetworkObject>().SpawnAsPlayerObject(manager.LocalClientId);
            Debug.Log("Player Spawned");
        }*/
    }
    /*void OnSessionOwnerPromoted(ulong ownerID)
    {
        if(ownerID == networkManager.LocalClientId)
        {
            networkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
    void ClientStarted()
    {
        Debug.Log("Client Started");
        SceneManager.LoadScene("Game");
    }
    void ServerStarted()
    {
        if (networkManager.IsServer)
        {
            networkManager.SceneManager.OnLoadComplete += OnSceneLoadComplete;
            networkManager.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (sceneName == "Game" && networkManager.IsClient && !networkManager.IsServer)
        {
            var player = Instantiate(networkManager.NetworkConfig.PlayerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }*/
}
