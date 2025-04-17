using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_StringBuilder", menuName = "Scriptables/Hooks/Others/Device Type", order = 0)]
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