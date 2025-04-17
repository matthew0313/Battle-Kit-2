using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player_Movements), typeof(Player_Animations), typeof(Player_Inventory))]
public class Player : Entity
{
    public Player_Movements movements { get; private set; }
    public Player_Animations animations { get; private set; }
    public Player_Inventory inventory { get; private set; }
    public static Player localPlayer { get; private set; }
    public NetworkVariable<Alliance> alliance = new NetworkVariable<Alliance>(Alliance.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private void Awake()
    {
        movements = GetComponent<Player_Movements>();
        animations = GetComponent<Player_Animations>();
        inventory = GetComponent<Player_Inventory>();
        movements.OnAwake();
        animations.OnAwake();
        inventory.OnAwake();
        //NetworkManager.OnConnectionEvent += OnConnection;
    }
    static Dictionary<ulong, Player> players = new();
    public static Player GetPlayerWithID(ulong clientID)
    {
        if(players.ContainsKey(clientID)) return players[clientID];
        return null;
    }
    private void Start()
    {
        movements.OnStart();
        inventory.OnStart();

        if (IsLocalPlayer)
        {
            localPlayer = this;
            foreach (var i in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ILocalPlayerSpawnCallbackReceiver>())
            {
                i.OnLocalPlayerSpawn(this);
            }
        }
        players.Add(OwnerClientId, this);
    }
    private void Update()
    {
        movements.OnUpdate();
        animations.OnUpdate();
        inventory.OnUpdate();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        //NetworkManager.OnConnectionEvent -= OnConnection;
    }
    public bool IsEnemy(Player other) => (alliance.Value & other.alliance.Value) == 0;

    /*void OnConnection(NetworkManager manager, ConnectionEventData data)
    {
        if(data.EventType == ConnectionEvent.ClientConnected && data.ClientId == manager.LocalClientId)
        {
            NetworkObject.enabled = true;
            NetworkObject.SpawnAsPlayerObject(data.ClientId);
        }
    }*/
}
public interface ILocalPlayerSpawnCallbackReceiver
{
    public void OnLocalPlayerSpawn(Player player);
}