using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

    [Header("Hooks")]
    [SerializeField] IRef<IHookProvider<float>> timeLeftHook;
    private void Update()
    {

    }
}
