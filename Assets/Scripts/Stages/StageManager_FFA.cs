using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class StageManager_FFA : StageManager
{
    [SerializeField] List<Transform> m_spawnPoints = new();
    public List<Transform> spawnPoints => m_spawnPoints;
}