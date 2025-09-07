using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "FlameBlessingData", menuName = "Scriptables/Aliment Settings/FlameBlessing")]
public class FlameBlessing_AlimentData : AlimentData
{

}
public class FlameBlessing_Aliment : Aliment
{
    new public readonly FlameBlessing_AlimentData settings;
    static FlameBlessing_AlimentData defaultSettings => AlimentManager.Instance.flameBlessingSettings;
    public FlameBlessing_Aliment() : base(defaultSettings)
    {
        this.settings = defaultSettings;
    }
    public FlameBlessing_Aliment(float duration, float burnDamage, float attackMultiplier, float defenseMultiplier) : base(duration, defaultSettings) 
    {
        this.settings = defaultSettings;
        this.burnDamage = burnDamage;
        this.attackMultiplier = attackMultiplier;
        this.defenseMultiplier = defenseMultiplier;
    }
    public override AlimentType type => AlimentType.FlameBlessing;
    public override LangText alimentDisplay => new()
    {
        en = $"Flame Blessing",
        kr = $"화염의 축복"
    };
    public float burnDamage = 1.0f, attackMultiplier = 1.0f, defenseMultiplier = 1.0f;
    const float burnTickRate = 1.0f;
    float counter = 0.0f;
    public override void OnAlimentAdd(HpComp affected)
    {
        base.OnAlimentAdd(affected);
        if(affected.TryGetComponent(out IAttackMultiplierAdjustable attack))
        {
            attack.attackMultiplier *= attackMultiplier;
        }
        if(affected.TryGetComponent(out IDefenseMultiplierAdjustable defense))
        {
            defense.defenseMultiplier *= defenseMultiplier;
        }
    }
    public override void OnAlimentUpdate(HpComp affected)
    {
        base.OnAlimentUpdate(affected);
        if (!affected.IsOwner) return;
        counter += Time.deltaTime;
        if(counter >= burnTickRate)
        {
            counter -= burnTickRate;
            affected.GetDamage(new()
            {
                amount = burnDamage,
                source = DamageSource.Other,
                hasColor = true,
                color = settings.alimentColor,
                ignoreDefense = true
            });
        }
    }
    public override void OnAlimentRemove(HpComp affected)
    {
        base.OnAlimentRemove(affected);
        if (affected.TryGetComponent(out IAttackMultiplierAdjustable attack))
        {
            attack.attackMultiplier /= attackMultiplier;
        }
        if (affected.TryGetComponent(out IDefenseMultiplierAdjustable defense))
        {
            defense.defenseMultiplier /= defenseMultiplier;
        }
    }
    public override Dictionary<string, string> SaveProperties()
    {
        var tmp = base.SaveProperties();
        tmp["burnDamage"] = burnDamage.ToString();
        tmp["attackMultiplier"] = attackMultiplier.ToString();
        tmp["defenseMultiplier"] = defenseMultiplier.ToString();
        return tmp;
    }
    public override void LoadProperties(Dictionary<string, string> data)
    {
        base.LoadProperties(data);
        if (data.TryGetValue("burnDamage", out string val)) burnDamage = float.Parse(val);
        if (data.TryGetValue("attackMultiplier", out string val2)) attackMultiplier = float.Parse(val2);
        if (data.TryGetValue("defenseMultiplier", out string val3)) defenseMultiplier = float.Parse(val3);

    }
}