using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public abstract class StageManager : NetworkBehaviour
{
    public static StageManager Instance { get; private set; }
    public StageManager() => Instance = this;
}