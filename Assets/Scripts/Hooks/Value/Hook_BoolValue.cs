using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_BoolValue", menuName = "Scriptables/Hooks/Values/Bool", order = 0)]
public class Hook_BoolValue : BoolHook
{
    [SerializeField] bool startValue = false;

    [Header("Debug")]
    [SerializeField] bool m_value = false;
    public override bool value => m_value;
    Action onValueChange;

    public override void AssignCallback(Action callback)
    {
        onValueChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        onValueChange -= callback;
    }

    public override void SetValue(bool value)
    {
        m_value = value;
        onValueChange?.Invoke();
    }
    protected override void OnReset()
    {
        m_value = startValue;
    }
}