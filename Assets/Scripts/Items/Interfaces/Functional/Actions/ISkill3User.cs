using System;
using Unity.Netcode;
using UnityEngine;

public interface ISkill3User
{
    public Sprite skill3Sprite { get; }
    public bool canUseSkill3 { get; }
    public float skill3CooldownBarFill { get; }
    public void Skill3Use(Player wielder);
}