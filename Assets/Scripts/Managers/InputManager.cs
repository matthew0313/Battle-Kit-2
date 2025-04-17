using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputManager() => Instance = this;

    [Header("General")]
    [SerializeField] InputActionReference mouseInput;

    [Header("Movement")]
    [SerializeField] InputActionReference movementInput;

    [Header("Inventory")]
    [SerializeField] InputActionReference beltSelectionInput;

    [Header("Items")]
    [SerializeField] InputActionReference itemUseInput;
    [SerializeField] InputActionReference itemUseHoldInput;
    [SerializeField] InputActionReference skill1UseInput, skill2UseInput;
    [SerializeField] InputActionReference reloadInput;

    [Header("Item Grab")]
    [SerializeField] InputActionReference itemGrabInput;
    [SerializeField] InputActionReference itemHalfGrabInput;

    public Vector2 MovementInput() => movementInput.action.ReadValue<Vector2>();
    public Vector2 MouseInput() => mouseInput.action.ReadValue<Vector2>();
    public bool BeltSelectionInput(out int beltIndex)
    {
        beltIndex = Mathf.FloorToInt(beltSelectionInput.action.ReadValue<float>());
        return beltSelectionInput.action.triggered;
    }
    public bool ItemUseInput() => itemUseInput.action.triggered;
    public bool ItemUseHoldInput() => itemUseHoldInput.action.ReadValue<bool>();
    public bool Skill1UseInput() => skill1UseInput.action.triggered;
    public bool Skill2UseInput() => skill2UseInput.action.triggered;
    public bool ItemGrabInput() => itemGrabInput.action.triggered;
    public bool ItemHalfGrabInput() => itemHalfGrabInput.action.triggered;
    public bool ReloadInput() => reloadInput.action.triggered;
}