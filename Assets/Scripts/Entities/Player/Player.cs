using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Player_Movements), typeof(Player_Skins), typeof(Player_Inventory))]
[RequireComponent(typeof(Player_HpComp))]
public class Player : Entity, ITimeSlowable
{
    public Animator anim;

    Player_Movements m_movements;
    public Player_Movements movements
    {
        get
        {
            if (m_movements == null) m_movements = GetComponent<Player_Movements>();
            return m_movements;
        }
    }
    Player_Skins m_animations;
    public Player_Skins animations
    {
        get
        {
            if(m_animations == null) m_animations = GetComponent<Player_Skins>();
            return m_animations;
        }
    }
    Player_Inventory m_inventory;
    public Player_Inventory inventory
    {
        get
        {
            if(m_inventory == null) m_inventory = GetComponent<Player_Inventory>();
            return m_inventory;
        }
    }
    Player_HpComp m_hpComp;
    public Player_HpComp hpComp
    {
        get
        {
            if(m_hpComp == null) m_hpComp = GetComponent<Player_HpComp>();
            return m_hpComp;
        }
    }
    public static Player localPlayer { get; private set; }

    public NetworkVariable<Alliance> alliance = new NetworkVariable<Alliance>(Alliance.None);
    public NetworkVariable<bool> combat = new NetworkVariable<bool>(true);
    
    [Rpc(SendTo.Authority)]
    public void SetAllianceRpc(Alliance alliance, RpcParams rpcParams)
    {
        if (rpcParams.Receive.SenderClientId != NetworkManager.Singleton.CurrentSessionOwner) return;
        this.alliance.Value = alliance;
    }
    [Rpc(SendTo.Authority)]
    public void SetCombatRpc(bool combat, RpcParams rpcParams)
    {
        if (rpcParams.Receive.SenderClientId != NetworkManager.Singleton.CurrentSessionOwner) return;
        this.combat.Value = combat;
    }
    public bool IsEnemy(Player other)
    {
        return other != this && this.combat.Value == true && (other.alliance.Value & this.alliance.Value) == 0;
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
        inventory.Init();
        if (IsLocalPlayer)
        {
            localPlayer = this;
            foreach(var i in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ILocalPlayerSpawnCallbackReceiver>())
            {
                i.OnLocalPlayerSpawn(this);
            }
        }
        players.Add(OwnerClientId, this);

        if (IsOwner) combat.Value = true;
    }
    public float timeScale { get; private set; } = 1.0f;
    public void SlowTime(float scale) => SlowTimeRpc(scale);
    [Rpc(SendTo.Authority)]
    public void SlowTimeRpc(float scale) => timeScale *= scale;
    public void RevertTime(float scale) => RevertTimeRpc(scale);
    [Rpc(SendTo.Authority)]
    public void RevertTimeRpc(float scale) => timeScale /= scale;
}
public interface ILocalPlayerSpawnCallbackReceiver
{
    public void OnLocalPlayerSpawn(Player player);
}
[System.Serializable]
public enum Alliance
{
    None = 0,
    TeamRed = 1,
    TeamBlue = 2,
    TeamGreen = 3,
    TeamYellow = 4
}