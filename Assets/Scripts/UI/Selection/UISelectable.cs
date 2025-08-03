using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UISelectable : MonoBehaviour
{
    public static List<UISelectable> selectables { get; } = new();
    protected virtual void OnEnable() => selectables.Add(this);
    protected virtual void OnDisable() => selectables.Remove(this);
    public virtual void OnSelect(UISelector selector) { }
    public virtual void OnSelectUpdate(UISelector selector) { }
    public virtual void OnDeselect(UISelector selector) { }
}