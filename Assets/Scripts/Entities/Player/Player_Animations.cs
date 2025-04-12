using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player_Animations : NetworkBehaviour
{
    Player origin;

    [SerializeField] PlayerSkin[] skins;
    PlayerSkin currentSkin;

    Animator anim => currentSkin.anim;
    NetworkVariable<int> currentSkinIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> moving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public void OnAwake()
    {
        origin = GetComponent<Player>();
        currentSkinIndex.OnValueChanged += UpdateSkin;
        UpdateSkin(-1, currentSkinIndex.Value);

        moving.OnValueChanged += SetMoving;
        SetMoving(false, moving.Value);
    }
    public void ChangeSkin(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= skins.Length) return;
        currentSkinIndex.Value = skinIndex;
    }
    void UpdateSkin(int prevIndex, int skinIndex)
    {
        if(skinIndex < 0 || skinIndex >= skins.Length) return;
        if (currentSkin != null) currentSkin.Hide();
        currentSkin = skins[skinIndex];
        currentSkin.Show();

        SetMoving(false, moving.Value);
    }
    public void OnUpdate()
    {
        if (IsOwner)
        {
            moving.Value = origin.movements.moveDir != Vector2.zero;
        }
    }
    readonly int movingID = Animator.StringToHash("1_Move");
    void SetMoving(bool prev, bool moving)
    {
        anim.SetBool(movingID, moving);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Player_Animations))]
public class Player_Animations_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Change Skin"))
        {
            Player_Animations tmp = target as Player_Animations;
            if (tmp.IsOwner)
            {
                tmp.ChangeSkin(1);
            }
        }
    }
}
#endif