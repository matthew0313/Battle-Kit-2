using System;
using Unity.Netcode;
using UnityEngine;

public interface IReloadable
{
    public bool canReload { get; }
    public float reloadBarFill { get; }
    public void Reload(Player wielder);
}