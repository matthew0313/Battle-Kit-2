using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HookedIndexEnabler : MonoBehaviour
{
    [SerializeField] IRef<IHookProvider<int>> hook;

    [Header("Display")]
    [SerializeField] HookedIndexEnablerPair[] m_targets;
    Dictionary<int, GameObject> targets;
    private void OnEnable()
    {
        if(targets == null)
        {
            targets = new();
            foreach(var i in m_targets)
            {
                targets.Add(i.index, i.target);
                i.target.SetActive(false);
            }
        }

        hook.I.AssignCallback(OnValueChange);
        OnValueChange();
    }
    private void OnDisable()
    {
        hook.I.RemoveCallback(OnValueChange);
    }
    GameObject currentActive;
    void OnValueChange()
    {
        if(targets.TryGetValue(hook.I.value, out GameObject target))
        {
            if (target == currentActive) return;
            if(currentActive != null) currentActive.SetActive(false);
            currentActive = target;
            currentActive.SetActive(true);
        }
    }
}
[System.Serializable]
public struct HookedIndexEnablerPair
{
    public int index;
    public GameObject target;
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HookedIndexEnablerPair))]
public class HookedIndexEnablerPair_Drawer : PropertyDrawer
{
    const int indexSize = 40;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = indexSize }, property.FindPropertyRelative("index"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position) { x = position.x + indexSize + 3, width = position.width - indexSize - 3 }, property.FindPropertyRelative("target"), GUIContent.none);
    }
}
#endif