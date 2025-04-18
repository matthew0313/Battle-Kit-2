using System;
using Unity.Netcode;
using UnityEngine;

public interface ISkill2User
{
    public Sprite skill2Sprite { get; }
    public bool canUseSkill2 { get; }
    public float skill2CooldownBarFill { get; }
    public void Skill2Use(Player wielder);
}