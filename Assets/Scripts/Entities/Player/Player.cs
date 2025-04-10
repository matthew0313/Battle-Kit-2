using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player_Movements))]
public class Player : Entity
{
    public Player_Movements movements { get; private set; }
    private void Awake()
    {
        movements = GetComponent<Player_Movements>();
        movements.OnAwake();
    }
    private void Start()
    {
        movements.OnStart();
    }
    private void Update()
    {
        movements.OnUpdate();
    }
}