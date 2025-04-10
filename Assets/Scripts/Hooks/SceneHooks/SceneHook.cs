using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class SceneHook<T> : MonoBehaviour, IHookProvider<T>
{
    public abstract T value { get; }
    public abstract void AssignCallback(Action callback);
    public abstract void RemoveCallback(Action callback);
    public abstract void SetValue(T value);
}