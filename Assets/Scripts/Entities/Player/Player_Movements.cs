using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NetworkTransform), typeof(Rigidbody2D))]
public class Player_Movements : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float knockbackResistance = 10.0f;

    [Header("Input")]
    [SerializeField] InputActionReference moveInput;

    Player origin;
    Rigidbody2D rb;
    public void OnAwake()
    {
        origin = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void OnStart()
    {
        if (!IsOwner) rb.bodyType = RigidbodyType2D.Kinematic;
        else rb.bodyType = RigidbodyType2D.Dynamic;
    }
    Vector2 knockback = Vector2.zero;
    public void OnUpdate()
    {
        if (IsOwner)
        {
            Move();
            UpdateKnockback();
        }
    }
    [Rpc(SendTo.Owner)]
    public void GetKnockbackRpc(Vector2 knockback)
    {
        this.knockback += knockback;
    }
    void Move()
    {
        rb.linearVelocity = moveInput.action.ReadValue<Vector2>() * moveSpeed + knockback;
    }
    void UpdateKnockback()
    {
        if (knockback == Vector2.zero) return;

        knockback -= knockback.normalized * Mathf.Min(knockbackResistance * Time.deltaTime, knockback.magnitude);
        if (knockback.magnitude < 0.1f) knockback = Vector2.zero;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Player_Movements))]
public class Player_Movements_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Receive Knockback (RPC)")) (target as Player_Movements).GetKnockbackRpc(Vector2.up * 5.0f);
    }
}
#endif