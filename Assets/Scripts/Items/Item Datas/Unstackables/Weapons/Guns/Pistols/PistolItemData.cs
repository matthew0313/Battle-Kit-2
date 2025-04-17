using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PistolItemData : GunItemData
{

}
public abstract class PistolItem : GunItem
{
    new public readonly PistolItemData data;
    new protected Pistol weapon => base.weapon as Pistol;
    protected PistolItem(PistolItemData data) : base(data)
    {

    }
}