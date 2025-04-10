using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player_Movements), typeof(Player_Inputs))]
public class Player : Entity
{
    public Player_Movements movements { get; private set; }
    public Player_Inputs inputs { get; private set; }
    private void Awake()
    {
        movements = GetComponent<Player_Movements>();
        inputs = GetComponent<Player_Inputs>();
        inputs.OnAwake();
        movements.OnAwake();
    }
    private void Update()
    {
        movements.OnUpdate();
    }
}