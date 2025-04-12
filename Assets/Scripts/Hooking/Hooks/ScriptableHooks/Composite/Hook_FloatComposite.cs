using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Hook_FloatComposite", menuName = "Scriptables/Hooks/Composites/Float", order = 0)]
public class Hook_FloatComposite : FloatHook
{
    [SerializeField] CompositeMode mode;
    [SerializeField] FloatHook[] hooks;
    [SerializeField] bool clamp01 = false;

    public override float value
    {
        get
        {
            float result = hooks[0].value;
            for(int i = 1; i < hooks.Length; i++)
            {
                switch (mode)
                {
                    case CompositeMode.Add:
                        result += hooks[i].value; break;
                    case CompositeMode.Subtract:
                        result -= hooks[i].value; break;
                    case CompositeMode.Multiply:
                        result *= hooks[i].value; break;
                    case CompositeMode.Divide:
                        result /= hooks[i].value; break;
                    case CompositeMode.Min:
                        result = Mathf.Min(result, hooks[i].value); break;
                    case CompositeMode.Max:
                        result = Mathf.Max(result, hooks[i].value); break;
                }
            }
            if (clamp01) result = Mathf.Clamp01(result);
            return result;
        }
    }

    public override void AssignCallback(Action callback)
    {
        foreach(var i in hooks)
        {
            i.AssignCallback(callback);
        }
    }

    public override void RemoveCallback(Action callback)
    {
        foreach(var i in hooks)
        {
            i.RemoveCallback(callback);
        }
    }

    public override void SetValue(float value)
    {
        Debug.LogWarning("Cannot set value of composite hooks");
    }
}
[System.Serializable]
public enum CompositeMode
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Min,
    Max
}