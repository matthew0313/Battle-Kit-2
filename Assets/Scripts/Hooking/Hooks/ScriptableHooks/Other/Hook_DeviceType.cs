using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Hook_DeviceType", menuName = "Scriptables/Hooks/Others/Device Type", order = 0)]
public class Hook_DeviceType : IntegerHook
{
    public override int value => (int)SystemInfo.deviceType;

    public override void AssignCallback(Action callback) { }

    public override void RemoveCallback(Action callback) { }

    public override void SetValue(int value)
    {
        Debug.Log("Cannot set value of Devicetype hook");
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Hook_DeviceType))]
public class Hook_DeviceType_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label(new GUIContent() { text = "0 = Unknown Device\n1 = Handheld\n2 = Console\n3 = Desktop" });
    }
}
#endif