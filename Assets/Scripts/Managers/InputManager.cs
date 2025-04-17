using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputManager() => Instance = this;

    [Header("Movement")]
    [SerializeField] InputActionReference movementInput;
    [SerializeField] InputActionReference aimInput;

    [Header("Inventory")]
    [SerializeField] InputActionReference beltSelectionInput;

    [Header("Items")]
    [SerializeField] InputActionReference itemUseInput;
    [SerializeField] InputActionReference skill1UseInput;

    public Vector2 MovementInput() => movementInput.action.ReadValue<Vector2>();
    public Vector2 AimInput() => Camera.main.ScreenToWorldPoint(aimInput.action.ReadValue<Vector2>());
    public bool BeltSelectionInput(out int beltIndex)
    {
        beltIndex = Mathf.FloorToInt(beltSelectionInput.action.ReadValue<float>());
        return beltSelectionInput.action.triggered;
    }
    public bool ItemUseInput() => itemUseInput.action.triggered;
    public bool Skill1UseInput() => skill1UseInput.action.triggered;
}