using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_StringValue", menuName = "Scriptables/Hooks/Values/String", order = 0)]
public class Hook_StringValue : StringHook
{
    [SerializeField] string startValue;

    [Header("Debug")]
    [SerializeField] string m_value;
    public override string value => m_value;
    Action onValueChange;

    public override void AssignCallback(Action callback)
    {
        onValueChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        onValueChange -= callback;
    }

    public override void SetValue(string value)
    {
        m_value = value;
        onValueChange?.Invoke();
    }
    protected override void OnReset()
    {
        m_value = startValue;
    }
}