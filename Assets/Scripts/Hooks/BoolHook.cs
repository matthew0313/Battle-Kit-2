using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BoolHook : Hook<bool>
{
    public void Invert()
    {
        SetValue(!value);
    }
}