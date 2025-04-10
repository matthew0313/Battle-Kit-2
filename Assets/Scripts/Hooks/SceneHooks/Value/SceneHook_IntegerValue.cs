using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneHook_IntegerValue : IntegerSceneHook
{
    [SerializeField] int m_value = 0;
    public override int value => m_value;
    Action onValueChange;
    private void OnValidate()
    {
        onValueChange?.Invoke();
    }
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
}