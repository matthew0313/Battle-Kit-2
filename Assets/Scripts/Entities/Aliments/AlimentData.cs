using System.Collections.Generic;
using Unity.Collections;
using System;
using UnityEngine;

public abstract class AlimentData : ScriptableObject
{
    [SerializeField] Color m_alimentColor;
    [SerializeField] Sprite m_alimentIcon;
    public Color alimentColor => m_alimentColor;
    public Sprite alimentIcon => m_alimentIcon;
}
public abstract class Aliment
{
    public readonly AlimentData settings;
    protected Aliment(AlimentData settings)
    {
        this.settings = settings;
    }
    protected Aliment(float duration, AlimentData settings)
    {
        this.settings = settings;
        SetDuration(duration);
    }
    public abstract AlimentType type { get; }
    public abstract LangText alimentDisplay { get; }
    public Action onAlimentDisplayChange;

    public ulong id;

    public float initialDuration = 3.0f;
    public float duration { get; private set; } = 1.0f;
    public void SetDuration(float duration)
    {
        if (affected != null && affected.IsOwner) affected.SetAlimentDurationRpc(id, duration);
        this.duration = duration;
        initialDuration = duration;
    }
    public virtual void AddTo(HpComp target)
    {
        target.AddAlimentInstanceRpc(MathUtilities.GenerateRandomID(), new()
        {
            type = type,
            properties = (NetworkDictionary)SaveProperties()
        }, new());
    }
    HpComp affected = null;
    public virtual void OnAlimentAdd(HpComp affected) { this.affected = affected; }
    public virtual void OnAlimentUpdate(HpComp affected)
    {
        duration = Mathf.Max(0.0f, duration - Time.deltaTime);
        if (affected.IsOwner && duration <= 0)
        {
            affected.DeleteAlimentInstanceRpc(id);
        }
    }
    public virtual void OnAlimentRemove(HpComp affected) { this.affected = null; }
    public virtual Dictionary<string, string> SaveProperties()
    {
        Dictionary<string, string> tmp = new();
        tmp["initialDuration"] = initialDuration.ToString();
        tmp["duration"] = duration.ToString();
        return tmp;
    }
    public virtual void LoadProperties(Dictionary<string, string> data)
    {
        if (data.TryGetValue("initialDuration", out string initialDuration)) this.initialDuration = float.Parse(initialDuration);
        if (data.TryGetValue("duration", out string duration)) this.duration = float.Parse(duration);
    }
    public static Aliment GetAliment(AlimentType type)
    {
        switch (type)
        {
            case AlimentType.Slowed: return new Slowed_Aliment();
            case AlimentType.Burning: return new Burning_Aliment();
            case AlimentType.FlameBlessing: return new FlameBlessing_Aliment();
            case AlimentType.AttackBuff: return new AttackBuff_Aliment();
            case AlimentType.DefenseBuff: return new DefenseBuff_Aliment();
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
    Slowed = 0,
    Burning = 1,
    FlameBlessing = 2,
    AttackBuff = 3,
    DefenseBuff = 4
}