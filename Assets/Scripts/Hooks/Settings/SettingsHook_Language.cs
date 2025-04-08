using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsHook_Language", menuName = "Scriptables/Hooks/Settings/Language", order = 0)]
public class SettingsHook_Language : IntegerHook
{
    public override int value => (int)Settings.language;

    public override void AssignCallback(Action callback)
    {
        Settings.onLanguageChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onLanguageChange -= callback;
    }

    public override void SetValue(int value)
    {
        Settings.language = (Language)value;
    }
}