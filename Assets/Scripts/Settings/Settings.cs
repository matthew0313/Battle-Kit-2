using System;
using Unity.Netcode;
using UnityEngine;

public static class Settings
{
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
}