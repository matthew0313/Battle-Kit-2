using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ControlElement
{
    public readonly Action onControlGain;
    public ControlElement(int priority, Action onControlGain, bool enabled = true)
    {
        m_enabled = enabled;
        m_priority = priority;
        this.onControlGain = onControlGain;
    }
    bool m_enabled;
    public bool enabled
    {
        get => m_enabled;
        set
        {
            if(m_enabled == value) return;

            m_enabled = value;
            onElementUpdate?.Invoke();
        }
    }
    int m_priority = 0;
    public int priority
    {
        get => m_priority;
        set
        {
            if (m_priority == value) return;

            m_priority = value;
            onElementUpdate?.Invoke();
        }
    }
    public Action onElementUpdate;
}
public class ControlElement<T>
{
    public readonly Action<T> onControlGain;
    public ControlElement(int priority, Action<T> onControlGain, bool enabled = true)
    {
        m_enabled = enabled;
        m_priority = priority;
        this.onControlGain = onControlGain;
    }
    bool m_enabled;
    public bool enabled
    {
        get => m_enabled;
        set
        {
            if (m_enabled == value) return;

            m_enabled = value;
            onElementUpdate?.Invoke();
        }
    }
    int m_priority = 0;
    public int priority
    {
        get => m_priority;
        set
        {
            if (m_priority == value) return;

            m_priority = value;
            onElementUpdate?.Invoke();
        }
    }
    public Action onElementUpdate;
}
public class ControlElement<T1, T2>
{
    public readonly Action<T1, T2> onControlGain;
    public ControlElement(int priority, Action<T1, T2> onControlGain, bool enabled = true)
    {
        m_enabled = enabled;
        m_priority = priority;
        this.onControlGain = onControlGain;
    }
    bool m_enabled;
    public bool enabled
    {
        get => m_enabled;
        set
        {
            if (m_enabled == value) return;

            m_enabled = value;
            onElementUpdate?.Invoke();
        }
    }
    int m_priority = 0;
    public int priority
    {
        get => m_priority;
        set
        {
            if (m_priority == value) return;

            m_priority = value;
            onElementUpdate?.Invoke();
        }
    }
    public Action onElementUpdate;
}
public class ControlElement<T1, T2, T3>
{
    public readonly Action<T1, T2, T3> onControlGain;
    public ControlElement(int priority, Action<T1, T2, T3> onControlGain, bool enabled = true)
    {
        m_enabled = enabled;
        m_priority = priority;
        this.onControlGain = onControlGain;
    }
    bool m_enabled;
    public bool enabled
    {
        get => m_enabled;
        set
        {
            if (m_enabled == value) return;

            m_enabled = value;
            onElementUpdate?.Invoke();
        }
    }
    int m_priority = 0;
    public int priority
    {
        get => m_priority;
        set
        {
            if (m_priority == value) return;

            m_priority = value;
            onElementUpdate?.Invoke();
        }
    }
    public Action onElementUpdate;
}