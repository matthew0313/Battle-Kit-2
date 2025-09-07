using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackBuffData", menuName = "Scriptables/Aliment Settings/AttackBuff")]
public class AttackBuff_AlimentData : AlimentData
{

}
public class AttackBuff_Aliment : Aliment
{
    new public readonly AttackBuff_AlimentData settings;
    static AttackBuff_AlimentData defaultSettings => AlimentManager.Instance.attackBuffSettings;
    public AttackBuff_Aliment() : base(defaultSettings)
    {
        this.settings = defaultSettings;
    }
    public AttackBuff_Aliment(float duration, float attackMultiplier) : base(duration, defaultSettings) 
    {
        this.settings = defaultSettings;
        this.attackMultiplier = attackMultiplier;
    }
    public override AlimentType type => AlimentType.AttackBuff;
    public override LangText alimentDisplay => new()
    {
        en = $"Attack Buff(+{Mathf.Floor((attackMultiplier - 1.0f) * 100.0f)}%)",
        kr = $"공격력 버프(+{Mathf.Floor((attackMultiplier - 1.0f) * 100.0f)}%)"
    };
    float m_attackMultiplier = 1.0f;
    public float attackMultiplier
    {
        get => m_attackMultiplier;
        set
        {
            m_attackMultiplier = value;
            onAlimentDisplayChange?.Invoke();
        }
    }
    IAttackMultiplierAdjustable adjusted;
    public override void OnAlimentAdd(HpComp affected)
    {
        base.OnAlimentAdd(affected);
        if (affected.IsOwner)
        {
            if (affected.TryGetComponent(out IAttackMultiplierAdjustable attack))
            {
                adjusted = attack;
                attack.attackMultiplier *= attackMultiplier;
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
                adjusted.attackMultiplier /= attackMultiplier;
                adjusted = null;
            }
        }
    }
    public override Dictionary<string, string> SaveProperties()
    {
        var tmp = base.SaveProperties();
        tmp["attackMultiplier"] = attackMultiplier.ToString();
        return tmp;
    }
    public override void LoadProperties(Dictionary<string, string> data)
    {
        base.LoadProperties(data);
        if (data.TryGetValue("attackMultiplier", out string val2)) attackMultiplier = float.Parse(val2);

    }
}