using System;
using Unity.Netcode;
using UnityEngine;

public interface IApplied
{
    public bool IsAppliableTo(Item other);
    public void ApplyTo(Item otehr);
}