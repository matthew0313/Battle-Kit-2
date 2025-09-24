using System;
using Unity.Netcode;
using UnityEngine;

public interface ILMBUser : IUsable
{
    public Sprite lmbSprite { get; }
    public bool canUseLmb { get; }
    public float lmbCooldownBarFill { get; }
    public void LMBUse(Player wielder, bool down);
}