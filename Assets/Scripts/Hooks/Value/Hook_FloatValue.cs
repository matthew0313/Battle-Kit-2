using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_FloatValue", menuName = "Scriptables/Hooks/Values/Float", order = 0)]
public class Hook_FloatValue : FloatHook
{
    [SerializeField] float startValue = 0.0f;

    [Header("Debug")]
    [SerializeField] float m_value = 0.0f;
    public override float value => m_value;
    Action onValueChange;

    public override void AssignCallback(Action callback)
    {
        onValueChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        onValueChange -= callback;
    }

    public override void SetValue(float value)
    {
        m_value = value;
        onValueChange?.Invoke();
    }
    protected override void OnReset()
    {
        m_value = startValue;
    }
}