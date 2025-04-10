using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class Player_Inputs : MonoBehaviour
{
    Player origin;
    PlayerInput input;
    public void OnAwake()
    {
        origin = GetComponent<Player>();
        input = GetComponent<PlayerInput>();
        if (!origin.IsOwner) input.enabled = false;
    }
}