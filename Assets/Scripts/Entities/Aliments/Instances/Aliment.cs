using System.Collections.Generic;
using Unity.Collections;
using System;
using UnityEngine;

public abstract class Aliment
{
    protected Aliment() { }
    protected Aliment(float duration) { this.duration = duration; }
    public abstract Aliment_Settings settings { get; }
    public abstract AlimentType type { get; }
    public abstract LangText alimentDisplay { get; }
    public Action onAlimentDisplayChange;

    public int id;

    public float initialDuration = 3.0f;
    float m_duration = 3.0f;
    public float duration
    {
        get => m_duration;
        set
        {
            m_duration = value;
            if (m_duration > initialDuration) initialDuration = m_duration;
        }
    }
    public virtual void AddTo(HpComp target)
    {
        target.AddAlimentInstanceRpc(UnityEngine.Random.Range(int.MinValue, int.MaxValue), new()
        {
            type = type,
            properties = (NetworkDictionary)SaveProperties()
        }, new());
    }
    public virtual void OnAlimentAdd(HpComp affected) { }
    public virtual void OnAlimentUpdate(HpComp affected)
    {
        Debug.Log(id);
        duration = Mathf.Max(0.0f, duration - Time.deltaTime);
        if (affected.IsOwner && duration <= 0)
        {
            affected.DeleteAlimentInstanceRpc(id);
        }
    }
    public virtual void OnAlimentRemove(HpComp affected) { }
    public virtual Dictionary<string, string> SaveProperties()
    {
        Dictionary<string, string> tmp = new();
        tmp["duration"] = duration.ToString();
        return tmp;
    }
    public virtual void LoadProperties(Dictionary<string, string> data)
    {
        if (data.TryGetValue("duration", out string duration)) this.duration = float.Parse(duration);
    }
    public static Aliment GetAliment(AlimentType type)
    {
        switch (type)
        {
            case AlimentType.Slowed: return new Slowed();
            default: return null;
        }
    }
    public AlimentInstanceData ToInstanceData()
    {
        return new()
        {
            type = type,
            properties = (NetworkDictionary)SaveProperties()
        };
    }
}
[System.Serializable]
public enum AlimentType
{
    Slowed = 0
}