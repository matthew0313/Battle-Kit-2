using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputManager() => Instance = this;

    [Header("Keybinds")]
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D, jump = KeyCode.Space;
    public KeyCode skill1Key = KeyCode.Q, skill2Key = KeyCode.E, reloadKey = KeyCode.R;
    public float MovementInput()
    {
        float tmp = 0.0f;
        if (Input.GetKey(moveRight)) tmp += 1.0f;
        if (Input.GetKey(moveLeft)) tmp -= 1.0f;
        return tmp;
    }
    public bool JumpInput() => Input.GetKeyDown(jump);
    public Vector2 MouseInput() => Input.mousePosition;
    public bool BeltSelectionInput(out int beltIndex)
    {
        for(int i = 0; i < 6; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                beltIndex = i; return true;
            }
        }
        beltIndex = -1; return false;
    }
    public bool ItemUseInput() => Input.GetMouseButtonDown(0) && UIScanner.ScanUI(MouseInput()).Count <= 0;
    public bool ItemUseHoldInput() => Input.GetMouseButton(0) && UIScanner.ScanUI(MouseInput()).Count <= 0;
    public bool Skill1UseInput() => Input.GetKeyDown(skill1Key);
    public bool Skill2UseInput() => Input.GetKeyDown(skill2Key);
    public bool ItemGrabInput() => Input.GetMouseButtonDown(0) && UIScanner.ScanUI(MouseInput()).Count > 0;
    public bool ItemUngrabInput()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld) return Input.GetMouseButtonUp(0);
        return Input.GetMouseButtonDown(0);
    }
    public bool ItemHalfGrabInput() => Input.GetMouseButtonDown(1) && UIScanner.ScanUI(MouseInput()).Count > 0;
    public bool ItemHalfUngrabInput() => Input.GetMouseButtonDown(1);
    public bool ReloadInput() => Input.GetKeyDown(reloadKey);
}