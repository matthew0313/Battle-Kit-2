using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class FloatHook : Hook<float>
{
    public void Add(float value)
    {
        SetValue(this.value + value);
    }
    public void Subtract(float value)
    {
        SetValue(this.value - value);
    }
    public void Multiply(float value)
    {
        SetValue(this.value * value);
    }
    public void Divide(float value)
    {
        SetValue(this.value / value);
    }
}