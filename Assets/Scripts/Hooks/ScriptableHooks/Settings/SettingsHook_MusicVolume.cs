using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsHook_MusicVolume", menuName = "Scriptables/Hooks/Settings/MusicVolume", order = 0)]
public class SettingsHook_MusicVolume : FloatHook
{
    public override float value => Settings.musicVolume;

    public override void AssignCallback(Action callback)
    {
        Settings.onMusicVolumeChange += callback;
    }

    public override void RemoveCallback(Action callback)
    {
        Settings.onMusicVolumeChange -= callback;
    }

    public override void SetValue(float value)
    {
        Settings.musicVolume = value;
    }
}