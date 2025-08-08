using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player_Cooldowns : NetworkBehaviour
{
    Player origin;
    public readonly List<CooldownSource> cooldowns = new();
    private void Awake()
    {
        origin = GetComponent<Player>();
    }
    readonly List<CooldownSource> removeQueue = new();
    private void Update()
    {
        if (!IsOwner) return;
        foreach(var i in cooldowns)
        {
            i.cooldownLeft = Mathf.Max(i.cooldownLeft - Time.deltaTime, 0.0f);
            if(i.cooldownLeft <= 0.0f) removeQueue.Add(i);
        }
        foreach (var i in removeQueue) cooldowns.Remove(i); removeQueue.Clear();
    }
    public void AddCooldown(CooldownSource data, float cooldown)
    {
        data.cooldown = cooldown;
        data.cooldownLeft = cooldown;
        if(!cooldowns.Contains(data)) cooldowns.Add(data);
    }
}
public struct CooldownInfo
{
    public float cooldown, cooldownLeft;
}