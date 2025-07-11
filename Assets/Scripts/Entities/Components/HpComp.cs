using Unity.Netcode;
using System;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using Unity.Collections;

public abstract class HpComp : NetworkBehaviour, IDamagable
{
    public NetworkBehaviour self => this;

    [SerializeField] float baseHp = 100.0f;
    public readonly NetworkVariable<float> maxHp = new NetworkVariable<float>(100.0f);
    public readonly NetworkVariable<float> hp = new NetworkVariable<float>(100.0f);
    public readonly NetworkVariable<bool> dead = new NetworkVariable<bool>(false);

    public event Action onHpChange;
    public event Action<DamageInfoWrapper> modifyDamageTaken;
    public event Action<DamageInfo> onDamage;

    public bool invincible = false;
    public readonly Dictionary<int, Aliment> aliments = new();
    public event Action onAlimentsUpdate;

    protected virtual void Awake()
    {
        hp.OnValueChanged += (a, b) => onHpChange?.Invoke();
        maxHp.OnValueChanged += (a, b) => onHpChange?.Invoke();
        NetworkManager.OnConnectionEvent += OnConnectionEvent;

        if (IsOwner)
        {
            maxHp.Value = baseHp;
            hp.Value = baseHp;
        }
    }
    readonly List<int> alimentRemoveQueue = new();
    protected virtual void Update()
    {
        if(alimentRemoveQueue.Count > 0)
        {
            foreach (var i in alimentRemoveQueue)
            {
                aliments[i].OnAlimentRemove(this);
                aliments.Remove(i);
            }
            alimentRemoveQueue.Clear();
            onAlimentsUpdate?.Invoke();
        }
        foreach(var i in aliments)
        {
            i.Value.OnAlimentUpdate(this);
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.OnConnectionEvent -= OnConnectionEvent;
    }
    void OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (!IsOwner) return;
        if(data.EventType == ConnectionEvent.ClientConnected)
        {
            foreach(var i in aliments)
            {
                AddAlimentInstanceRpc(i.Key, i.Value.ToInstanceData(), RpcTarget.Single(data.ClientId, RpcTargetUse.Temp));
            }
        }
    }

    [Rpc(SendTo.Authority)]
    public void GetDamageRpc(DamageInfo damage)
    {
        if (dead.Value == true || invincible) return;

        DamageInfoWrapper wrap = new() { damage = damage };
        modifyDamageTaken?.Invoke(wrap);
        damage = wrap.damage;

        float amount = Mathf.Min(hp.Value, damage.amount);
        hp.Value -= amount;
        onDamage?.Invoke(damage);

        if (hp.Value <= 0)
        {
            dead.Value = true;
            OnDeath();
        }
    }
    public void GetDamage(DamageInfo damage) => GetDamageRpc(damage);
    protected virtual void OnDeath() { }
    public void Revive()
    {
        if (!IsOwner) return;
        hp.Value = maxHp.Value;
        dead.Value = false;
        OnRevive();
    }
    protected virtual void OnRevive() { }
    public abstract bool IsEnemy(Player other);

    [Rpc(SendTo.Authority)]
    public void AddAlimentRpc(AlimentInstanceData info)
    {
        Aliment tmp = info.ToAliment();
        tmp.AddTo(this);
    }
    public void AddAliment(Aliment aliment)
    {
        if (IsOwner)
        {
            aliment.AddTo(this);
        }
        else
        {
            AddAlimentRpc(aliment.ToInstanceData());
        }
    }
    [Rpc(SendTo.Everyone, AllowTargetOverride = true)]
    public void AddAlimentInstanceRpc(int id, AlimentInstanceData info, RpcParams rpcParams)
    {
        if (aliments.ContainsKey(id))
        {
            aliments[id].OnAlimentRemove(this);
            aliments.Remove(id);
        }

        Aliment tmp = info.ToAliment();
        tmp.id = id;
        tmp.OnAlimentAdd(this);
        aliments[id] = tmp;
        onAlimentsUpdate?.Invoke();
    }
    [Rpc(SendTo.Everyone)]
    public void DeleteAlimentInstanceRpc(int id)
    {
        if (!aliments.ContainsKey(id) || alimentRemoveQueue.Contains(id)) return;
        alimentRemoveQueue.Add(id);
    }

    public IEnumerable<Aliment> GetAliments()
    {
        foreach (var i in aliments) yield return i.Value;
    }
}
public class DamageInfoWrapper
{
    public DamageInfo damage;
}