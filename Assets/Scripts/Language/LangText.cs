using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct LangText
{
    [TextArea] public string en, kr;
    public string text
    {
        get
        {
            switch (Settings.language)
            {
                case Language.English: return en;
                case Language.Korean: return kr;
                default: return en;
            }
        }
    }
}