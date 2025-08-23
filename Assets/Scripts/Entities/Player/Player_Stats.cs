using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player_Stats : NetworkBehaviour, ISpeedMultiplierAdjustable, IAttackMultiplierAdjustable, IDefenseMultiplierAdjustable
{
    public float speedMultiplier { get; set; } = 1.0f;
    public float attackMultiplier { get; set; } = 1.0f;
    public float defenseMultiplier { get; set; } = 1.0f;
}