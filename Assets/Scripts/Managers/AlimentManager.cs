using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class AlimentManager : MonoBehaviour
{
    public static AlimentManager Instance { get; private set; }
    public AlimentManager() => Instance = this;

    [Header("Settings")]
    [SerializeField] BurningData m_burningSettings;
    [SerializeField] SlowedData m_slowedSettings;
    public BurningData burningSettings => m_burningSettings;
    public SlowedData slowedSettings => m_slowedSettings;
}