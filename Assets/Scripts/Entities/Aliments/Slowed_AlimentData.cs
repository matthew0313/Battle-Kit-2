using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "SlowedData", menuName = "Scriptables/Aliment Settings/Slowed")]
public class Slowed_AlimentData : AlimentData
{

}
public class Slowed_Aliment : Aliment
{
    public Slowed_Aliment() { }
    public Slowed_Aliment(float duration, float slowPercentage) : base(duration) { this.slowPercentage = slowPercentage; }
    public override AlimentData settings => AlimentManager.Instance.slowedSettings;
    public override AlimentType type => AlimentType.Slowed;
    public override LangText alimentDisplay => new()
    {
        en = $"Slowed({slowPercentage}%)",
        kr = $"감속({slowPercentage}%)"
    };
    float m_slowPercentage = 0.0f;
    public float slowPercentage
    {
        get => m_slowPercentage;
        set
        {
            m_slowPercentage = value;
            onAlimentDisplayChange?.Invoke();
        }
    }

    ISpeedMultiplierAdjustable adjusted;
    public override void OnAlimentAdd(HpComp affected)
    {
        base.OnAlimentAdd(affected);
        if (affected.IsOwner)
        {
            if(affected.TryGetComponent(out ISpeedMultiplierAdjustable speed))
            {
                adjusted = speed;
                adjusted.speedMultiplier *= (1.0f - slowPercentage / 100.0f);
            }
        }
    }
    public override void OnAlimentRemove(HpComp affected)
    {
        base.OnAlimentRemove(affected);
        if (affected.IsOwner)
        {
            if(adjusted != null)
            {
                adjusted.speedMultiplier /= (1.0f - slowPercentage / 100.0f);
            }
        }
    }
    public override Dictionary<string, string> SaveProperties()
    {
        var tmp = base.SaveProperties();
        tmp["slowPercentage"] = slowPercentage.ToString();
        return tmp;
    }
    public override void LoadProperties(Dictionary<string, string> data)
    {
        base.LoadProperties(data);
        if(data.TryGetValue("slowPercentage", out string val)) slowPercentage = float.Parse(val);
    }
}