using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<InputManager>("InputManager"));
        DontDestroyOnLoad(Instance.gameObject);
    }

    PlayerInputs playerInputs;
    [Header("Select Move")]
    [SerializeField] float holdThreshold = 0.5f;
    [SerializeField] float holdRepeatTime = 0.1f;
    private void Awake()
    {
        playerInputs = new PlayerInputs();
    }
    private void OnEnable()
    {
        playerInputs.Enable();
    }
    private void OnDisable()
    {
        playerInputs.Disable();
    }
    Vector2 selectMoveVal = Vector2.zero;
    Vector2 lastMoveDir = Vector2.zero;
    float holdCounter = 0.0f, holdRepeatCounter = 0.0f;
    private void Update()
    {
        Vector2 tmp = playerInputs.Default.SelectMove.ReadValue<Vector2>();
        Vector2 moveDir = Vector2.zero;
        if (tmp.x > 0) moveDir.x += 1;
        else if (tmp.x < 0) moveDir.x -= 1;
        else if (tmp.y > 0) moveDir.y += 1;
        else if (tmp.y < 0) moveDir.y -= 1;
        selectMoveVal = Vector2.zero;
        if (moveDir != Vector2.zero)
        {
            if (moveDir != lastMoveDir)
            {
                selectMoveVal = moveDir;
                holdCounter = 0.0f; holdRepeatCounter = 0.0f;
            }
            else
            {
                holdCounter += Time.deltaTime;
                if (holdCounter >= holdThreshold)
                {
                    holdRepeatCounter = Mathf.Max(holdRepeatCounter - Time.deltaTime, 0);
                    if (holdRepeatCounter <= 0.0f)
                    {
                        holdRepeatCounter = holdRepeatTime;
                        selectMoveVal = moveDir;
                    }
                }
            }
        }
        lastMoveDir = moveDir;
    }
    public float MovementInput() => playerInputs.Default.Movement.ReadValue<float>();
    public bool JumpInput()
    {
        if (DeviceManager.deviceType == DeviceType.Console && UIManager.Instance.isTabOpen) return false;
        return playerInputs.Default.Jump.triggered;
    }
    public bool ItemUseInput(out bool down)
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if(UIScanner.ScanUI(Input.mousePosition).Count > 0 || InventorySlotUIHandler.Instance.grabbing)
            {
                down = false;
                return false;
            }
        }
        down = playerInputs.Default.ItemUse.triggered;
        return playerInputs.Default.ItemUse.IsPressed();
    }
    public bool Skill1UseInput() => !UIManager.Instance.isTabOpen && playerInputs.Default.Skill1Use.triggered;
    public bool Skill2UseInput() => !UIManager.Instance.isTabOpen && playerInputs.Default.Skill2Use.triggered;
    public bool ReloadInput() => !UIManager.Instance.isTabOpen && playerInputs.Default.Reload.triggered;
    public Vector2 SelectMoveInput() => selectMoveVal;
    public bool SelectInput() => playerInputs.Default.Select.triggered;
    public bool CancelInput() => playerInputs.Default.Cancel.triggered;
    public bool InventoryOpenInput() => playerInputs.Default.InventoryOpen.triggered;
}
public enum ButtonMode
{
    None,
    Down,
    Pressed,
    Up
}