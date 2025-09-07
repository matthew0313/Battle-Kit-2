using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseBuffData", menuName = "Scriptables/Aliment Settings/DefenseBuff")]
public class DefenseBuff_AlimentData : AlimentData
{

}
public class DefenseBuff_Aliment : Aliment
{
    new public readonly DefenseBuff_AlimentData settings;
    static DefenseBuff_AlimentData defaultSettings => AlimentManager.Instance.defenseBuffSettings;
    public DefenseBuff_Aliment() : base(defaultSettings)
    {
        this.settings = defaultSettings;
    }
    public DefenseBuff_Aliment(float duration, float defenseMultiplier) : base(duration, defaultSettings) 
    {
        this.settings = defaultSettings;
        this.defenseMultiplier = defenseMultiplier;
    }
    public override AlimentType type => AlimentType.DefenseBuff;
    public override LangText alimentDisplay => new()
    {
        en = $"Defense Buff(+{Mathf.Floor((defenseMultiplier - 1.0f) * 100.0f)}%)",
        kr = $"방어력 버프(+{Mathf.Floor((defenseMultiplier - 1.0f) * 100.0f)}%)"
    };
    float m_defenseMultiplier = 1.0f;
    public float defenseMultiplier
    {
        get => m_defenseMultiplier;
        set
        {
            m_defenseMultiplier = value;
            onAlimentDisplayChange?.Invoke();
        }
    }
    IDefenseMultiplierAdjustable adjusted;
    public override void OnAlimentAdd(HpComp affected)
    {
        base.OnAlimentAdd(affected);
        if (affected.IsOwner)
        {
            if (affected.TryGetComponent(out IDefenseMultiplierAdjustable defense))
            {
                adjusted = defense;
                defense.defenseMultiplier *= defenseMultiplier;
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
                adjusted.defenseMultiplier /= defenseMultiplier;
                adjusted = null;
            }
        }
    }
    public override Dictionary<string, string> SaveProperties()
    {
        var tmp = base.SaveProperties();
        tmp["defenseMultiplier"] = defenseMultiplier.ToString();
        return tmp;
    }
    public override void LoadProperties(Dictionary<string, string> data)
    {
        base.LoadProperties(data);
        if (data.TryGetValue("defenseMultiplier", out string val2)) defenseMultiplier = float.Parse(val2);

    }
}