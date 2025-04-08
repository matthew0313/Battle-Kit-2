using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMachine
{
    readonly Action defaultControl;
    public ControlMachine(Action defaultControl = null)
    {
        this.defaultControl = defaultControl;
    }

    List<ControlElement> controls = new();
    void UpdateControlList()
    {
        controls.Sort((a, b) =>
        {
            if (!a.enabled)
            {
                if (!b.enabled) return 0;
                else return 1;
            }
            if (!b.enabled) return -1;

            return b.priority.CompareTo(a.priority);
        });
        if (controls.Count > 0 && controls[0].enabled) controls[0].onControlGain.Invoke();
        else defaultControl?.Invoke();
    }
    public void AddControls(params ControlElement[] controls)
    {
        foreach(var i in controls)
        {
            this.controls.Add(i);
            i.onElementUpdate += UpdateControlList;
        }
        UpdateControlList();
    }
    public void RemoveControl(ControlElement control)
    {
        controls.Remove(control);
        control.onElementUpdate -= UpdateControlList;

        UpdateControlList();
    }
}
public class ControlMachine<T>
{
    readonly T arg;
    readonly Action<T> defaultControl;
    public ControlMachine(T arg, Action<T> defaultControl = null)
    {
        this.arg = arg;
        this.defaultControl = defaultControl;
    }

    List<ControlElement<T>> controls = new();
    void UpdateControlList()
    {
        controls.Sort((a, b) =>
        {
            if (!a.enabled)
            {
                if (!b.enabled) return 0;
                else return 1;
            }
            if (!b.enabled) return -1;

            return b.priority.CompareTo(a.priority);
        });
        if (controls.Count > 0 && controls[0].enabled) controls[0].onControlGain.Invoke(arg);
        else defaultControl?.Invoke(arg);
    }
    public void AddControls(params ControlElement<T>[] controls)
    {
        foreach (var i in controls)
        {
            this.controls.Add(i);
            i.onElementUpdate += UpdateControlList;
        }
        UpdateControlList();
    }
    public void RemoveControl(ControlElement<T> control)
    {
        controls.Remove(control);
        control.onElementUpdate -= UpdateControlList;

        UpdateControlList();
    }
}
public class ControlMachine<T1, T2>
{
    readonly T1 arg1;
    readonly T2 arg2;
    readonly Action<T1, T2> defaultControl;
    public ControlMachine(T1 arg1, T2 arg2, Action<T1, T2> defaultControl = null)
    {
        this.arg1 = arg1;
        this.arg2 = arg2;
        this.defaultControl = defaultControl;
    }

    List<ControlElement<T1, T2>> controls = new();
    void UpdateControlList()
    {
        controls.Sort((a, b) =>
        {
            if (!a.enabled)
            {
                if (!b.enabled) return 0;
                else return 1;
            }
            if (!b.enabled) return -1;

            return b.priority.CompareTo(a.priority);
        });
        if (controls.Count > 0 && controls[0].enabled) controls[0].onControlGain.Invoke(arg1, arg2);
        else defaultControl?.Invoke(arg1, arg2);
    }
    public void AddControls(params ControlElement<T1, T2>[] controls)
    {
        foreach (var i in controls)
        {
            this.controls.Add(i);
            i.onElementUpdate += UpdateControlList;
        }
        UpdateControlList();
    }
    public void RemoveControl(ControlElement<T1, T2> control)
    {
        controls.Remove(control);
        control.onElementUpdate -= UpdateControlList;

        UpdateControlList();
    }
}
public class ControlMachine<T1, T2, T3>
{
    readonly T1 arg1;
    readonly T2 arg2;
    readonly T3 arg3;
    readonly Action<T1, T2, T3> defaultControl;
    public ControlMachine(T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> defaultControl = null)
    {
        this.arg1 = arg1;
        this.arg2 = arg2;
        this.arg3 = arg3;
        this.defaultControl = defaultControl;
    }

    List<ControlElement<T1, T2, T3>> controls = new();
    void UpdateControlList()
    {
        controls.Sort((a, b) =>
        {
            if (!a.enabled)
            {
                if (!b.enabled) return 0;
                else return 1;
            }
            if (!b.enabled) return -1;

            return b.priority.CompareTo(a.priority);
        });
        if (controls.Count > 0 && controls[0].enabled) controls[0].onControlGain.Invoke(arg1, arg2, arg3);
        else defaultControl?.Invoke(arg1, arg2, arg3);
    }
    public void AddControls(params ControlElement<T1, T2, T3>[] controls)
    {
        foreach (var i in controls)
        {
            this.controls.Add(i);
            i.onElementUpdate += UpdateControlList;
        }
        UpdateControlList();
    }
    public void RemoveControl(ControlElement<T1, T2, T3> control)
    {
        controls.Remove(control);
        control.onElementUpdate -= UpdateControlList;

        UpdateControlList();
    }
}