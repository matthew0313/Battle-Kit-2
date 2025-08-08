using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using NUnit.Framework.Constraints;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

    [Header("Player Spawning")]
    [SerializeField] Player playerPrefab;
    [SerializeField] Transform lobbySpawnPoint;
    readonly List<Player> players = new();


    [Header("Rounds")]
    [SerializeField] int roundTime = 240;
    [SerializeField] int intermissionTime = 30;
    [SerializeField] List<GameMode> gameModeCycle = new();
    [SerializeField] List<string> ffaStages = new(), teamStages = new();
    public readonly NetworkVariable<int> timeLeft = new(30);
    public readonly NetworkVariable<bool> roundInProgress = new(false);
    public readonly NetworkVariable<GameMode> currentGameMode = new(GameMode.FreeForAll);
    public readonly NetworkVariable<int> gameModeCycleIndex = new(0);

    float counter = 0.0f;

    private void OnEnable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnStageLoadComplete;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnStageLoadComplete;
    }
    private void OnStageLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner || loadSceneMode == LoadSceneMode.Single) return;
        if (currentGameMode.Value == GameMode.FreeForAll)
        {
            StageManager_FFA stageManager = StageManager.Instance as StageManager_FFA;
            foreach (var i in players)
            {
                i.SetPVPOnRpc(true);
                i.movements.TeleportRpc(stageManager.spawnPoints[UnityEngine.Random.Range(0, stageManager.spawnPoints.Count)].position);
            }
        }
        else if (currentGameMode.Value == GameMode.TeamMatch)
        {
            StageManager_TM stageManager = StageManager.Instance as StageManager_TM;
            List<Player> teamRed = players.ToList(), teamBlue = new();
            for (int i = 0; i < teamRed.Count / 2; i++)
            {
                int index = UnityEngine.Random.Range(0, teamRed.Count);
                teamBlue.Add(teamRed[index]);
                teamRed.RemoveAt(index);
            }
            foreach (var i in teamRed)
            {
                i.SetTeamRpc(Team.TeamRed);
                i.SetPVPOnRpc(true);
                i.movements.TeleportRpc(stageManager.redSpawn.position);
            }
            foreach (var i in teamBlue)
            {
                i.SetTeamRpc(Team.TeamBlue);
                i.SetPVPOnRpc(true);
                i.movements.TeleportRpc(stageManager.blueSpawn.position);
            }
        }
    }

    private void Start()
    {
        if (IsOwner) NetworkObject.Spawn();
        Player player = Instantiate(playerPrefab, lobbySpawnPoint.position, Quaternion.identity);
        player.NetworkObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, true);
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (timeLeft.Value > 0)
        {
            counter += Time.deltaTime;
            if (counter >= 1.0f)
            {
                counter -= 1.0f;
                if(--timeLeft.Value <= 0)
                {
                    if (roundInProgress.Value) EndRound();
                    else StartRound();
                }
            }
        }
    }
    void StartRound()
    {
        if (players.Count < 2)
        {
            timeLeft.Value = intermissionTime; return;
        }
        timeLeft.Value = roundTime;
        roundInProgress.Value = true;
        currentGameMode.Value = gameModeCycle[gameModeCycleIndex.Value];
        gameModeCycleIndex.Value = (gameModeCycleIndex.Value + 1) % gameModeCycle.Count;
        List<string> stageScenes = currentGameMode.Value == GameMode.FreeForAll ? ffaStages : teamStages;
        NetworkManager.Singleton.SceneManager.LoadScene(stageScenes[UnityEngine.Random.Range(0, stageScenes.Count)], LoadSceneMode.Additive);
    }
    void EndRound()
    {
        timeLeft.Value = intermissionTime;
        roundInProgress.Value = false;
        foreach (var i in players)
        {
            if (i.PVPOn.Value)
            {
                i.SetTeamRpc(Team.None);
                i.SetPVPOnRpc(false);
                i.movements.TeleportRpc(lobbySpawnPoint.position);
                i.inventory.ClearRpc();
            }
        }
        List<Scene> loadedScenes = NetworkManager.Singleton.SceneManager.GetSynchronizedScenes();
        NetworkManager.Singleton.SceneManager.UnloadScene(loadedScenes[1]);
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
[System.Serializable]
public enum GameMode
{
    FreeForAll = 0,
    TeamMatch = 1
}
