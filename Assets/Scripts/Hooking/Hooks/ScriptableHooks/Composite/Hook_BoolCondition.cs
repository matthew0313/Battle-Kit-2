using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_StringBuilder", menuName = "Scriptables/Hooks/Composites/Bool Condition", order = 0)]
public class Hook_BoolCondition : BoolHook
{
    public override bool value => throw new NotImplementedException();

    public override void AssignCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void RemoveCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(bool value)
    {
        Debug.LogWarning("Cannot set value of composite hooks");
    }
}