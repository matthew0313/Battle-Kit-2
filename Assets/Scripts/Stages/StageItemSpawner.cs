using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

public class StageItemSpawner : NetworkBehaviour
{
    [SerializeField] LootGen lootGen;
    [SerializeField] int spawnTimeMin = 10, spawnTimeMax = 30;
    public readonly NetworkVariable<int> spawnTimeLeft = new(2);
    float counter = 0.0f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
    private void Update()
    {
        if (!IsOwner) return;
        counter += Time.deltaTime;
        if (counter >= 1.0f)
        {
            counter -= 1.0f;
            if (--spawnTimeLeft.Value <= 0)
            {
                spawnTimeLeft.Value = UnityEngine.Random.Range(spawnTimeMin, spawnTimeMax + 1);
                Spawn();
            }
        }
    }
    void Spawn()
    {
        foreach(var i in lootGen.GetLoot())
        {
            DroppedItem tmp = DroppedItem.Create();
            tmp.transform.position = transform.position;
            tmp.NetworkObject.Spawn();
            tmp.Set(i.item, i.count, 1.0f);
        }
    }
}