using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class IntegerSceneHook : SceneHook<int>
{
    public void Add(int value)
    {
        SetValue(this.value + value);
    }
    public void Subtract(int value)
    {
        SetValue(this.value - value);
    }
    public void Multiply(int value)
    {
        SetValue(this.value * value);
    }
    public void Divide(int value)
    {
        SetValue(this.value / value);
    }
    public void Remainder(int value)
    {
        SetValue(this.value % value);
    }
}