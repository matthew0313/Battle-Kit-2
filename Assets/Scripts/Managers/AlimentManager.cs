using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class AlimentManager : MonoBehaviour
{
    public static AlimentManager Instance { get; private set; }
    public AlimentManager() => Instance = this;

    [Header("Settings")]
    [SerializeField] Burning_AlimentData m_burningSettings;
    [SerializeField] Slowed_AlimentData m_slowedSettings;
    [SerializeField] FlameBlessing_AlimentData m_flameBlessingSettings;
    [SerializeField] AttackBuff_AlimentData m_attackBuffSettings;
    [SerializeField] DefenseBuff_AlimentData m_defenseBuffSettings;
    public Burning_AlimentData burningSettings => m_burningSettings;
    public Slowed_AlimentData slowedSettings => m_slowedSettings;
    public FlameBlessing_AlimentData flameBlessingSettings => m_flameBlessingSettings;
    public AttackBuff_AlimentData attackBuffSettings => m_attackBuffSettings;
    public DefenseBuff_AlimentData defenseBuffSettings => m_defenseBuffSettings;
}