using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NetworkTransform), typeof(Rigidbody2D))]
public class Player_Movements : NetworkBehaviour, IKnockable, ISpeedMultiplierAdjustable
{
    Player origin;
    public NetworkTransform networkTransform { get; private set; }
    public Rigidbody2D rb { get; private set; }

    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float knockbackResistance = 10.0f;

    [Header("Rotation")]
    [SerializeField] Transform rotator;
    [SerializeField] Transform flipped;

    public Vector2 moveDir { get; private set; } = Vector2.zero;

    public bool mouseLocked = false;
    public ControlMachine<Player_Movements> mouseLockedControl;

    public bool canMove = true;
    public ControlMachine<Player_Movements> canMoveControl;

    public bool canRotate = true;
    public ControlMachine<Player_Movements> canRotateControl;

    public float speedMultiplier { get; set; } = 1.0f;
    public void OnAwake()
    {
        origin = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        networkTransform = GetComponent<NetworkTransform>();
        mouseLockedControl = new(this, (movements) => movements.mouseLocked = false);
        canMoveControl = new(this, (movements) => movements.canMove = true);
        canRotateControl = new(this, (movements) => movements.canRotate = true);
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
    public bool IsEnemy(Player other)
    {
        return origin.IsEnemy(other);
    }

    public void GetKnockback(KnockbackInfo knockback)
    {
        GetKnockbackRpc(knockback);
    }
    [Rpc(SendTo.Authority)]
    public void GetKnockbackRpc(KnockbackInfo knockback)
    {
        this.knockback += knockback.value;
    }
    [Rpc(SendTo.Authority)]
    public void TeleportRpc(Vector2 position) => transform.position = position;
    void Move()
    {
        Vector2 dir = Vector2.zero;

        if (canMove && origin.hpComp.dead.Value == false)
        {
            moveDir = InputManager.Instance.MovementInput();
            dir += moveDir * moveSpeed * speedMultiplier;
            if (!mouseLocked)
            {
                if (moveDir.x > 0)
                {
                    flipped.localScale = new Vector2(-1.0f, 1.0f);
                }
                else if (moveDir.x < 0)
                {
                    flipped.localScale = new Vector2(1.0f, 1.0f);
                }
            }
        }
        dir += knockback;

        rb.linearVelocity = dir;
    }
    void Rotate()
    {
        if (origin.hpComp.dead.Value == true) return;

        Vector2 tmp = Camera.main.ScreenToWorldPoint(InputManager.Instance.MouseInput()) - rotator.position;
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
        if (GUILayout.Button("Receive Knockback (RPC)")) (target as Player_Movements).GetKnockbackRpc(new() { value = Vector2.up * 5.0f });
    }
}
#endif