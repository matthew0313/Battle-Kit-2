using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneHook_StringValue : StringSceneHook
{
    [SerializeField] string m_value = string.Empty;
    public override string value => m_value;
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

    public override void SetValue(string value)
    {
        m_value = value;
        onValueChange?.Invoke();
    }
}