using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Player_Movements), typeof(Player_Animations), typeof(Player_Inventory))]
[RequireComponent(typeof(Player_HpComp))]
public class Player : Entity
{
    public Player_Movements movements { get; private set; }
    public Player_Animations animations { get; private set; }
    public Player_Inventory inventory { get; private set; }
    public Player_HpComp hpComp { get; private set; }
    public static Player localPlayer { get; private set; }
    public NetworkVariable<Alliance> alliance = new NetworkVariable<Alliance>(Alliance.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> combat = new NetworkVariable<bool>(true);
    
    [Rpc(SendTo.Owner)]
    public void SetAllianceRpc(Alliance alliance, RpcParams rpcParams)
    {
        if (rpcParams.Receive.SenderClientId != NetworkManager.Singleton.CurrentSessionOwner) return;
        this.alliance.Value = alliance;
    }
    [Rpc(SendTo.Owner)]
    public void SetCombatRpc(bool combat, RpcParams rpcParams)
    {
        if (rpcParams.Receive.SenderClientId != NetworkManager.Singleton.CurrentSessionOwner) return;
        this.combat.Value = combat;
    }
    private void Awake()
    {
        movements = GetComponent<Player_Movements>();
        animations = GetComponent<Player_Animations>();
        inventory = GetComponent<Player_Inventory>();
        hpComp = GetComponent<Player_HpComp>();
        movements.OnAwake();
        animations.OnAwake();
        inventory.OnAwake();
        hpComp.OnAwake();
        //NetworkManager.OnConnectionEvent += OnConnection;
    }
    static Dictionary<ulong, Player> players = new();
    public static Player GetPlayerWithID(ulong clientID)
    {
        if(players.ContainsKey(clientID)) return players[clientID];
        return null;
    }
    public static int GetPlayerCount() => players.Count;
    public static IEnumerable<Player> GetPlayers()
    {
        foreach(var i in players.Values) yield return i;
    }
    private void Start()
    {
        movements.OnStart();
        inventory.OnStart();
        hpComp.OnStart();

        if (IsLocalPlayer)
        {
            localPlayer = this;
            foreach (var i in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ILocalPlayerSpawnCallbackReceiver>())
            {
                i.OnLocalPlayerSpawn(this);
            }
        }
        players.Add(OwnerClientId, this);

        if (IsOwner) combat.Value = true;
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