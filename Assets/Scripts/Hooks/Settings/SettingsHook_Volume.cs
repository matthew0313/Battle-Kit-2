using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsHook_Volume", menuName = "Scriptables/Hooks/Settings/Volume", order = 0)]
public class SettingsHook_Volume : FloatHook
{
    public override float value => Settings.volume;

    public override void AssignCallback(Action callback)
    {
        Settings.onVolumeChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onVolumeChange -= callback;
    }

    public override void SetValue(float value)
    {
        Settings.volume = value;
    }
}