using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "BurningData", menuName = "Scriptables/Aliment Settings/Burning")]
public class Burning_AlimentData : AlimentData
{

}
public class Burning_Aliment : Aliment
{
    public Burning_Aliment() : base() { }
    public Burning_Aliment(float duration, float burnDamage) : base(duration) { this.burnDamage = burnDamage; }
    public override AlimentData settings => AlimentManager.Instance.burningSettings;
    public override AlimentType type => AlimentType.Burning;
    public override LangText alimentDisplay => new()
    {
        en = $"Burning({burnDamage})",
        kr = $"화상({burnDamage})"
    };
    float m_burnDamage = 1.0f;
    public float burnDamage
    {
        get => m_burnDamage;
        set
        {
            m_burnDamage = value;
            onAlimentDisplayChange?.Invoke();
        }
    }
    public override void AddTo(HpComp target)
    {
        foreach(var i in target.GetAliments())
        {
            if(i is Burning_Aliment other)
            {
                other.burnDamage += burnDamage;
                other.duration = Mathf.Max(other.duration, duration);
                return;
            }
        }
        base.AddTo(target);
    }
    const float burnTickRate = 0.5f;
    float counter = 0.0f;
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
    public override Dictionary<string, string> SaveProperties()
    {
        var tmp = base.SaveProperties();
        tmp["burnDamage"] = burnDamage.ToString();
        return tmp;
    }
    public override void LoadProperties(Dictionary<string, string> data)
    {
        base.LoadProperties(data);
        if (data.TryGetValue("burnDamage", out string val)) burnDamage = float.Parse(val);
    }
}