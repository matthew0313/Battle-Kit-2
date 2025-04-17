using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OwnerOnlyRigidbody2D : NetworkBehaviour
{
    public Rigidbody2D rb { get; private set; }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        if (!IsOwner) rb.bodyType = RigidbodyType2D.Kinematic;
    }
}