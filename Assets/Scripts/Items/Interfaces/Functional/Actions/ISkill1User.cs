using System;
using Unity.Netcode;
using UnityEngine;

public interface ISkill1User : IUsable
{
    public Sprite skill1Sprite { get; }
    public bool canUseSkill1 { get; }
    public float skill1CooldownBarFill { get; }
    public void Skill1Use(Player wielder);
}