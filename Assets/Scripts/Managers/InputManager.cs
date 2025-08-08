using System;
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
    [SerializeField] float selectHoldThreshold = 0.5f;
    [SerializeField] float selectHoldRepeatTime = 0.1f;
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
    private void Update()
    {
        UpdateSelectionMoveInput();
    }
    Vector2 selectMoveVal = Vector2.zero;
    Vector2 selectLastMoveDir = Vector2.zero;
    float selectHoldCounter = 0.0f, selectHoldRepeatCounter = 0.0f;
    void UpdateSelectionMoveInput()
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
            if (moveDir != selectLastMoveDir)
            {
                selectMoveVal = moveDir;
                selectHoldCounter = 0.0f; selectHoldRepeatCounter = 0.0f;
            }
            else
            {
                selectHoldCounter += Time.deltaTime;
                if (selectHoldCounter >= selectHoldThreshold)
                {
                    selectHoldRepeatCounter -= Time.deltaTime;
                    if (selectHoldRepeatCounter <= 0.0f)
                    {
                        selectHoldRepeatCounter += selectHoldRepeatTime;
                        selectMoveVal = moveDir;
                    }
                }
            }
        }
        selectLastMoveDir = moveDir;
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
    public bool DropInput() => playerInputs.Default.Drop.triggered;
}
public enum ButtonMode
{
    None,
    Down,
    Pressed,
    Up
}