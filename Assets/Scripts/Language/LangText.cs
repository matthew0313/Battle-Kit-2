using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct LangText
{
    public string en, kr;
    public static implicit operator string(LangText text)
    {
        switch (Settings.language)
        {
            case Language.English: return text.en;
            case Language.Korean : return text.kr;
            default: return text.en;
        }
    }
}