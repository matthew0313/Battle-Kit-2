using System;
using Unity.Netcode;
using UnityEngine;

public interface ILMBUser
{
    public void LMBUse(Player wielder, bool down);
}