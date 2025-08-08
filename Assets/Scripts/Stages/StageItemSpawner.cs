using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

public class StageItemSpawner : NetworkBehaviour
{
    [SerializeField] LootGen lootGen;
    [SerializeField] Transform itemSpawnPoint;
    [SerializeField] int spawnTimeMin = 10, spawnTimeMax = 30;
    public readonly NetworkVariable<int> spawnTimeLeft = new(2);
    float counter = 0.0f;
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
            tmp.transform.position = itemSpawnPoint.position;
            tmp.NetworkObject.Spawn();
            tmp.Set(i.item, i.count, 1.0f);
        }
    }
}