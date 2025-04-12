using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IHookProvider<T>
{
    public T value { get; }
    public void AssignCallback(Action callback);
    public void RemoveCallback(Action callback);
    public void SetValue(T value);
}