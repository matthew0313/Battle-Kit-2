using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class SwordItemData : MeleeItemData
{

}
public abstract class SwordItem : MeleeItem
{
    new public readonly SwordItemData data;
    new protected Sword weapon => base.weapon as Sword;
    protected SwordItem(SwordItemData data) : base(data)
    {
        this.data = data;
    }
}