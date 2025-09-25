using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Sound))]
public class Sound_Editor : Editor
{
    public VisualTreeAsset treeAsset;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement element = treeAsset.CloneTree();
        return element;
    }
    [MenuItem("Assets/Create Sound")]
    public static void CreateSound()
    {
        AudioClip clip = Selection.activeObject as AudioClip;
        Sound asset = CreateInstance<Sound>();
        asset.audioSource = clip;

        AssetDatabase.CreateAsset(asset, AssetDatabase.GetAssetPath(clip) + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

    }
    [MenuItem("Assets/Create Sound", true)]
    public static bool CreateSoundValidation()
    {
        return Selection.activeObject is AudioClip;
    }
}