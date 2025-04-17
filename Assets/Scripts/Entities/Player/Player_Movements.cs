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

    [Header("Rotation")]
    [SerializeField] Transform rotator;
    [SerializeField] Transform flipped;

    Player origin;
    Rigidbody2D rb;
    public bool mouseLocked = false;
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
            Rotate();
            UpdateKnockback();
        }
    }
    [Rpc(SendTo.Owner)]
    public void GetKnockbackRpc(Vector2 knockback)
    {
        this.knockback += knockback;
    }
    public Vector2 moveDir { get; private set; } = Vector2.zero;
    void Move()
    {
        moveDir = InputManager.Instance.MovementInput();
        rb.linearVelocity = moveDir * moveSpeed + knockback;
        if (!mouseLocked)
        {
            if(moveDir.x > 0)
            {
                flipped.localScale = new Vector2(-1.0f, 1.0f);
            }
            else if(moveDir.x < 0)
            {
                flipped.localScale = new Vector2(1.0f, 1.0f);
            }
        }
    }
    void Rotate()
    {
        Vector2 tmp = InputManager.Instance.AimInput() - (Vector2)rotator.position;
        float deg = Mathf.Atan2(tmp.y, tmp.x) * Mathf.Rad2Deg;
        if (deg < 0.0f) deg += 360.0f;
        if (deg > 90.0f && deg < 270.0f)
        {
            rotator.localScale = new Vector2(1.0f, -1.0f);
        }
        else
        {
            rotator.localScale = new Vector2(1.0f, 1.0f);
        }
        rotator.rotation = Quaternion.Euler(0, 0, deg);
        if (mouseLocked)
        {
            if (tmp.x > 0)
            {
                flipped.localScale = new Vector2(-1.0f, 1.0f);
            }
            else if (tmp.x < 0)
            {
                flipped.localScale = new Vector2(1.0f, 1.0f);
            }
        }
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