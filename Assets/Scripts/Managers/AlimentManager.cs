using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class AlimentManager : MonoBehaviour
{
    public static AlimentManager Instance { get; private set; }
    public AlimentManager() => Instance = this;

    [Header("Settings")]
    [SerializeField] Burning_Settings m_burningSettings;
    [SerializeField] Slowed_Settings m_slowedSettings;
    public Burning_Settings burningSettings => m_burningSettings;
    public Slowed_Settings slowedSettings => m_slowedSettings;
}