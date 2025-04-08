using System;
using Unity.Netcode;
using UnityEngine;

public static class Settings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Init()
    {
        m_language = Language.Korean;
        m_volume = 1.0f;
        m_musicVolume = 1.0f;
    }

    static Language m_language;
    public static Language language
    {
        get => m_language;
        set
        {
            if (m_language == value) return;
            m_language = value;
            onLanguageChange?.Invoke();
        }
    }
    public static Action onLanguageChange;
    static float m_volume;
    public static float volume
    {
        get => m_volume;
        set
        {
            m_volume = value;
            onVolumeChange?.Invoke();
        }
    }
    public static Action onVolumeChange;
    static float m_musicVolume;
    public static float musicVolume
    {
        get => m_musicVolume;
        set
        {
            m_musicVolume = value;
            onMusicVolumeChange?.Invoke();
        }
    }
    public static Action onMusicVolumeChange;
    public static SettingsSave Save()
    {
        return new()
        {
            language = m_language,
            volume = m_volume,
            musicVolume = m_musicVolume
        };
    }
    public static void Load(SettingsSave save)
    {
        m_language = save.language;
        m_volume = save.volume;
        m_musicVolume = save.musicVolume;
    }
}
[System.Serializable]
public class SettingsSave
{
    public Language language = Language.Korean;
    public float volume = 1.0f, musicVolume = 1.0f;
}