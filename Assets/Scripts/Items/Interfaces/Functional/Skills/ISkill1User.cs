using System;
using Unity.Netcode;
using UnityEngine;

public interface ISkill1User
{
    public Sprite skill1Sprite { get; }
    public bool canUseSkill1 { get; }
    public float skill1CooldownBarFill { get; }
    public void Skill1Use();
}