using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Cooldown Source", menuName = "Scriptables/Cooldown Source")]
public class CooldownSource : ResetScriptable
{
    public float cooldownLeft = 0.0f, cooldown = 10.0f;

    protected override void OnReset()
    {
        cooldownLeft = 0.0f;
        cooldown = 10.0f;
    }
}