using System;
using Unity.Netcode;
using UnityEngine;

public interface IAmmoCountDisplayed
{
    public Action onAmmoCountDisplayUpdate { get; set; }
    public string ammoCountText { get; }
    public Sprite ammoIcon { get; }
}