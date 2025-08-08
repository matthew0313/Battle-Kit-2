using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class StageManager_TM : StageManager
{
    [SerializeField] Transform m_redSpawn, m_blueSpawn;
    public Transform redSpawn => m_redSpawn;
    public Transform blueSpawn => m_blueSpawn;
}