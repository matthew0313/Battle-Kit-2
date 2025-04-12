using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player_Movements), typeof(Player_Animations))]
public class Player : Entity
{
    public Player_Movements movements { get; private set; }
    public Player_Animations animations { get; private set; }
    private void Awake()
    {
        movements = GetComponent<Player_Movements>();
        animations = GetComponent<Player_Animations>();
        movements.OnAwake();
        animations.OnAwake();
    }
    private void Start()
    {
        movements.OnStart();
    }
    private void Update()
    {
        movements.OnUpdate();
        animations.OnUpdate();
    }
}