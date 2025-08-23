using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Player_Movements), typeof(Player_Skins), typeof(Player_Inventory))]
[RequireComponent(typeof(Player_HpComp), typeof(Player_Cooldowns), typeof(Player_Stats))]
public class Player : Entity
{
    public Animator anim;
    [SerializeField] Animator m_localAnim;
    public Animator localAnim => m_localAnim;

    public Player_Movements movements { get; private set; }
    public Player_Skins skins { get; private set; }
    public Player_Inventory inventory { get; private set; }
    public Player_HpComp hpComp { get; private set; }
    public Player_Cooldowns cooldowns { get; private set; }
    public Player_Stats stats { get; private set; }
    public static Player localPlayer { get; private set; }

    public readonly NetworkVariable<Team> team = new(Team.None);
    public readonly NetworkVariable<bool> PVPOn = new(false);
    public readonly NetworkVariable<bool> inRound = new(false);
    
    [Rpc(SendTo.Authority)]
    public void SetTeamRpc(Team team) => this.team.Value = team;
    [Rpc(SendTo.Authority)]
    public void SetPVPOnRpc(bool PVPOn) => this.PVPOn.Value = PVPOn;
    [Rpc(SendTo.Authority)]
    public void SetInRoundRpc(bool inRound) => this.inRound.Value = inRound;
    public Relation GetRelation(Entity other)
    {
        if (other == null) return Relation.None;
        if (other == this) return Relation.Self;
        if(other is Player player)
        {
            if (!PVPOn.Value || !player.PVPOn.Value) return Relation.None;
            if (team.Value != Team.None && team.Value == player.team.Value) return Relation.Ally;
            return Relation.Enemy;
        }
        return Relation.None;
    }
    static Dictionary<ulong, Player> players = new();
    public static Player GetPlayer(ulong clientID) => players[clientID];
    private void Awake()
    {
        movements = GetComponent<Player_Movements>();
        skins = GetComponent<Player_Skins>();
        inventory = GetComponent<Player_Inventory>();
        hpComp = GetComponent<Player_HpComp>();
        cooldowns = GetComponent<Player_Cooldowns>();
        stats = GetComponent<Player_Stats>();
    }
    private void Start()
    {
        players.Add(OwnerClientId, this);
        GameManager.Instance.AddPlayer(this);
        inventory.Init();
        if (IsLocalPlayer)
        {
            localPlayer = this;
            foreach(var i in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ILocalPlayerSpawnCallbackReceiver>())
            {
                i.OnLocalPlayerSpawn(this);
            }
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (players.ContainsKey(OwnerClientId)) players.Remove(OwnerClientId);
        GameManager.Instance.RemovePlayer(this);
    }
}
public interface ILocalPlayerSpawnCallbackReceiver
{
    public void OnLocalPlayerSpawn(Player player);
}
[System.Serializable]
public enum Team
{
    None = 0,
    TeamRed = 1,
    TeamBlue = 2,
    TeamGreen = 3,
    TeamYellow = 4
}
[System.Serializable]
public enum Relation
{
    None = 0,
    Enemy = 1,
    Ally = 2,
    Self = 3
}