using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class MyNetworkManager : MonoBehaviour
{
    public static MyNetworkManager Instance { get; private set; }
    public MyNetworkManager() => Instance = this;
    public NetworkManager networkManager { get; private set; }
    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
    }
}
