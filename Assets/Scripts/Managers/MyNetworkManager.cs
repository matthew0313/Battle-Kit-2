using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkManager))]
public class MyNetworkManager : MonoBehaviour
{
    public static MyNetworkManager Instance { get; private set; }
    public MyNetworkManager() => Instance = this;
    public NetworkManager networkManager { get; private set; }
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        networkManager.NetworkConfig.AutoSpawnPlayerPrefabClientSide = false;
    }
    private void OnEnable()
    {
        networkManager.OnServerStarted += ServerStarted;
        SessionManager.Instance.onSessionChange += OnSessionChange;
    }
    private void OnDisable()
    {
        networkManager.OnServerStarted -= ServerStarted;
        SessionManager.Instance.onSessionChange -= OnSessionChange;
    }
    ISession currentSession;
    void OnSessionChange(ISession session)
    {
        if (session != null)
        {
            if (session.IsHost) networkManager.StartHost();
            else networkManager.StartClient();
        }
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
    }
}
