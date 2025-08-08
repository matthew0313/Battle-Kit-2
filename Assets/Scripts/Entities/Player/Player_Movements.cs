using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NetworkTransform), typeof(Rigidbody2D))]
public class Player_Movements : NetworkBehaviour, IKnockable, ISpeedMultiplierAdjustable
{
    public Entity owner => origin;
    Player origin;
    public NetworkTransform networkTransform { get; private set; }
    public Rigidbody2D rb { get; private set; }

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float jumpPower = 10.0f;
    [SerializeField] Vector2 groundScanOffset, groundScanSize;
    [SerializeField] LayerMask groundMask;
    public bool isGrounded => Physics2D.OverlapBox((Vector2)transform.position + groundScanOffset, groundScanSize, 0.0f, groundMask);

    [Header("Rotation")]
    [SerializeField] Transform flipped;

    public Vector2 moveDir { get; private set; } = Vector2.zero;

    public bool canMove { get; private set; } = true;
    public ControlMachine<bool> canMoveControl;

    public bool canRotate { get; private set; } = true;
    public ControlMachine<bool> canRotateControl;

    public float speedMultiplier { get; set; } = 1.0f;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + groundScanOffset, groundScanSize);
    }
    private void Awake()
    {
        origin = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        networkTransform = GetComponent<NetworkTransform>();
        canMoveControl = new(value => canMove = value, true);
        canRotateControl = new(value => canRotate = value, true);
    }
    readonly int isFallingID = Animator.StringToHash("IsFalling"), isGroundedID = Animator.StringToHash("IsGrounded");
    float knockbackX = 0.0f;
    bool doubleJumped = false;
    private void Update()
    {
        if (IsOwner)
        {
            if (isGrounded) doubleJumped = false;
            if(canMove) Move();
            UpdateKnockback();
        }
    }
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            origin.anim.SetBool(isFallingID, rb.linearVelocityY < -0.1f);
            origin.anim.SetBool(isGroundedID, isGrounded);
        }
    }

    public void GetKnockback(KnockbackInfo knockback)
    {
        GetKnockbackRpc(knockback);
    }
    [Rpc(SendTo.Authority)]
    public void GetKnockbackRpc(KnockbackInfo knockback)
    {
        rb.linearVelocityY = knockback.value.y;
        knockbackX = knockback.value.x;
    }
    [Rpc(SendTo.Authority)]
    public void TeleportRpc(Vector2 position) => transform.position = position;
    readonly int isMovingID = Animator.StringToHash("IsMoving");
    readonly int jumpID = Animator.StringToHash("Jump");
    void Move()
    {
        rb.linearVelocityX = InputManager.Instance.MovementInput() * moveSpeed * speedMultiplier + knockbackX;
        origin.anim.SetBool(isMovingID, InputManager.Instance.MovementInput() != 0);
        if (isGrounded)
        {
            if (InputManager.Instance.JumpInput()) Jump();
            if (canRotate)
            {
                if (InputManager.Instance.MovementInput() > 0)
                {
                    flipped.rotation = Quaternion.Euler(0, 0, 0);
                    flipped.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                if (InputManager.Instance.MovementInput() < 0)
                {
                    flipped.rotation = Quaternion.Euler(0, 0, 180);
                    flipped.localScale = new Vector3(1.0f, -1.0f, 1.0f);
                }
            }
        }
        else 
        {
            if (!doubleJumped && InputManager.Instance.JumpInput())
            {
                doubleJumped = true;
                Jump();
            }
        }
    }
    void Jump()
    {
        rb.linearVelocityY = jumpPower;
        origin.anim.SetBool(isFallingID, false);
        origin.anim.SetTrigger(jumpID);
    }
    void UpdateKnockback()
    {
        if (knockbackX < 0.0f) knockbackX = Mathf.Min(knockbackX + Time.deltaTime * rb.gravityScale, 0.0f);
        if (knockbackX > 0.0f) knockbackX = Mathf.Max(knockbackX - Time.deltaTime * rb.gravityScale, 0.0f);
    }
    private void OnDisable()
    {
        rb.linearVelocityX = 0.0f;
        knockbackX = 0.0f;
        origin.anim.SetBool(isMovingID, false);
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