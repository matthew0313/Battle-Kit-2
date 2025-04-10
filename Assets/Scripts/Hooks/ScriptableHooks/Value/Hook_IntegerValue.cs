using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_IntegerValue", menuName = "Scriptables/Hooks/Values/Integer", order = 0)]
public class Hook_IntegerValue : IntegerHook
{
    [SerializeField] int startValue = 0;

    [Header("Debug")]
    [SerializeField] int m_value = 0;
    public override int value => m_value;
    Action onValueChange;

    public override void AssignCallback(Action callback)
    {
        onValueChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        onValueChange -= callback;
    }

    public override void SetValue(int value)
    {
        m_value = value;
        onValueChange?.Invoke();
    }
    protected override void OnReset()
    {
        m_value = startValue;
    }
}