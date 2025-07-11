using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player_Skins : NetworkBehaviour
{
    Player origin;

    [SerializeField] Transform skinAnchor;
    [SerializeField] PlayerSkin currentSkin;
    [SerializeField] PlayerSkin[] skins;
    readonly NetworkVariable<int> currentSkinIndex = new(0);
    private void Awake()
    {
        origin = GetComponent<Player>();
        currentSkinIndex.OnValueChanged += OnCurrentSkinIndexChange;
        origin.anim = currentSkin.anim;
    }
    public void ChangeSkin(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= skins.Length) return;
        currentSkinIndex.Value = skinIndex;
    }
    void OnCurrentSkinIndexChange(int prev, int skinIndex)
    {
        if(skinIndex < 0 || skinIndex >= skins.Length) return;

        Destroy(currentSkin);
        PlayerSkin skin = Instantiate(skins[skinIndex], skinAnchor);
        skin.transform.localPosition = Vector2.zero;
        origin.anim = skin.anim;

        currentSkin = skin;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Player_Skins))]
public class Player_Skins_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Change Skin"))
        {
            Player_Skins tmp = target as Player_Skins;
            if (tmp.IsOwner)
            {
                tmp.ChangeSkin(1);
            }
        }
    }
}
#endif